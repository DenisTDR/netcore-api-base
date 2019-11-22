using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using API.Base.Web.Base.ApiBuilder;
using API.Base.Web.Base.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace API.Base.Web.Base.Controllers
{
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public class ModelMetadataController : DiController
    {
        private readonly ILightTranslationsRepository _translationsRepository;
        private static readonly object LockObject = new object();
        private static JObject _cache;

        private static readonly ConcurrentDictionary<string, bool> NotTranslations =
            new ConcurrentDictionary<string, bool>();

        public ModelMetadataController(ILightTranslationsRepository translationsRepository)
        {
            _translationsRepository = translationsRepository;
        }

        [HttpGet]
        public IActionResult GetTranslatedSwaggerJson([FromQuery] bool clearCache = false)
        {
            EnsureLoaded(clearCache);
            return Ok(_cache);
        }

        [HttpGet]
        public IActionResult GetNotTranslatedFields([FromQuery] bool clearCache = false)
        {
            EnsureLoaded(clearCache);
            return Ok(NotTranslations.Select(x => x.Key));
        }
        
        public static void ClearCache()
        {
            lock (LockObject)
            {
                _cache = null;
                NotTranslations.Clear();
            }
        }

        private void EnsureLoaded(bool clearCache)
        {
            lock (LockObject)
            {
                if (clearCache)
                {
                    _cache = null;
                    NotTranslations.Clear();
                }

                if (_cache != null) return;

                var swaggerSpecs = ServiceProvider.GetService<IOptions<SwaggerSpecs>>().Value;
                var path = swaggerSpecs.RouteTemplate.Replace("{documentName}", swaggerSpecs.Name);
                var url2 = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
                var jsonUrl = url2 + path;

                var client = new HttpClient();

                var result = client.GetStringAsync(jsonUrl).Result;

                var jObject = (JObject) JsonConvert.DeserializeObject(result);
                Translate(jObject);
                _cache = jObject;
            }
        }


        private void Translate(JObject jObject)
        {
            var defJObject = (JObject) jObject["definitions"];
            foreach (var keyValuePair in defJObject)
            {
                var modelDef = (JObject) keyValuePair.Value;
                var modelProps = (JObject) modelDef["properties"];
                foreach (var modelProp in modelProps)
                {
                    TakeCareOfProp((JObject) modelProp.Value, "hint");
                    TakeCareOfProp((JObject) modelProp.Value, "placeholder");
                    TakeCareOfProp((JObject) modelProp.Value, "category");
                    var validators = modelProp.Value["validators"];
                    if (validators != null && validators is JArray jArrayValidators)
                    {
                        foreach (var jArrayValidator in jArrayValidators)
                        {
                            TakeCareOfProp((JObject) jArrayValidator, "message");
                        }
                    }
                }
            }
        }

        private void TakeCareOfProp(JObject modelProp, string propKey)
        {
            var oldJValue = modelProp[propKey];
            if (oldJValue == null || !(oldJValue is JValue value) || value.Type != JTokenType.String) return;

            var oldValue = modelProp[propKey].ToString();
            var translation = _translationsRepository.Translate(oldValue).Result;
            if (translation != null)
            {
                if (translation == "empty-string")
                {
                    translation = null;
                }
                modelProp[propKey] = new JValue(translation);
            }
            else
            {
                NotTranslations[oldValue] = true;
            }
        }
    }
}
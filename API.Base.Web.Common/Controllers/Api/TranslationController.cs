using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using API.Base.Web.Base.Controllers.Api;
using API.Base.Web.Common.Data;
using API.Base.Web.Common.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Base.Web.Common.Controllers.Api
{
    public class TranslationController : ApiController
    {
        private readonly ITranslationsRepository _translationsRepository;

        public TranslationController(ITranslationsRepository translationsRepository)
        {
            _translationsRepository = translationsRepository;
        }

        [AllowAnonymous]
        [HttpGet("{lang}")]
        [ProducesResponseType(typeof(IEnumerable<TranslationViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll([FromRoute] string lang)
        {
            return Ok(await _translationsRepository.GetAllForLanguage(lang));
        }
    }
}
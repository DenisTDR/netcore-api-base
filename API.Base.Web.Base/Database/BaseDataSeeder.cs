using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Base.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace API.Base.Web.Base.Database
{
    public class BaseDataSeeder<TDst> : IDataSeeder where TDst : BaseDataSeedType, new()
    {
        protected readonly IDataLayer DataLayer;
        protected readonly RoleManager<Role> RoleManager;
        protected readonly string FileName = "data-seed.json";


        protected TDst SeedingData { get; set; }

        public BaseDataSeeder(IServiceProvider serviceProvider)
        {
            DataLayer = serviceProvider.GetService<IDataLayer>();
            RoleManager = serviceProvider.GetService<RoleManager<Role>>();
        }

        public virtual async Task LoadSeedData()
        {
            if (!File.Exists(FileName))
            {
                throw new Exception("Data seed file does not exist.");
            }

            var str = await new StreamReader(new FileStream(FileName, FileMode.Open, FileAccess.Read)).ReadToEndAsync();
            SeedingData = JsonConvert.DeserializeObject<TDst>(str);
        }

        public async Task MigrateDatabase()
        {
            await DataLayer.MigrateDatabase();
        }

        public virtual async Task LoadSeed()
        {
            await LoadSeedData();
            await SeedRoles();
        }

        public virtual async Task SeedRoles()
        {
            foreach (var role in SeedingData.Roles)
            {
                if (await RoleManager.FindByNameAsync(role) == null)
                {
                    await RoleManager.CreateAsync(new Role {Name = role});
                }
            }
        }

        private List<string> _ParseGenerateSeedParameters(string param)
        {
            if (param == "true")
            {
                return null;
            }
            else
            {
                var entities = param.Split(',');
                for (var i = 0; i < entities.Length; i++)
                {
                    entities[i] = entities[i].ToCamelCase();
                    entities[i] = char.ToUpper(entities[i][0]) + entities[i].Substring(1);
                }

                return new List<string>(entities);
            }
        }

        public async Task<string> SeedToString(string param)
        {
            var entities = _ParseGenerateSeedParameters(param);
            await SetSeedingData(entities);
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            return JsonConvert.SerializeObject(SeedingData, jsonSettings);
        }

        public async Task EnsureMigrated()
        {
            await DataLayer.EnsureMigrated();
        }

        public virtual async Task SeedToFile(string param)
        {
            var json = await SeedToString(param);
            var sw = new StreamWriter(FileName, false);
            await sw.WriteAsync(json);
            sw.Close();

            Console.WriteLine("Updated {0}", FileName);
        }

#pragma warning disable 1998
        protected virtual async Task SetSeedingData(List<string> entities = null)
#pragma warning restore 1998
        {
            SeedingData = new TDst
            {
                Roles = RoleManager.Roles.OrderBy(x => x.Name).Select(x => x.Name).ToList()
            };
        }
    }

    public class BaseDataSeedType
    {
        public IEnumerable<string> Roles { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using API.Base.Web.Base.Extensions;
using API.Base.Web.Base.Misc;
using API.Base.Web.Base.Models;
using API.Base.Web.Base.Models.Entities;
using API.Base.Web.Base.Models.EntityMaps;
using API.Base.Web.Base.Models.ViewModels;

namespace API.Base.Web.Base.ApiBuilder
{
    public class EntityViewModelApiBuilderHelper
    {
        public IEnumerable<EntityTypePairConfiguration> ProcessEntities(List<ApiSpecifications> apiSpecificationsList)
        {
            var configs = new List<EntityTypePairConfiguration>();
            var entityTypes = new List<Type>();
            var entityTypeConfigurationTypes = new List<Type>();
            var entityViewModelMapTypes = new List<Type>();
            var viewModelTypes = new List<Type>();

            foreach (var apiSpecifications in apiSpecificationsList)
            {
                entityTypes.AddRange(apiSpecifications.Assembly.GetSubTypes(typeof(Entity))
                    .Where(type => !type.GetCustomAttributes<NotMappedAttribute>().Any())
                    .Where(type => !type.IsAbstract));


                entityTypeConfigurationTypes.AddRange(apiSpecifications.Assembly
                    .GetSubTypes(typeof(EntityTypeConfiguration<>)).Where(type => !type.IsAbstract));


                entityViewModelMapTypes.AddRange(apiSpecifications.Assembly
                    .GetSubTypes(typeof(EntityViewModelMap<,>)).Where(type => !type.IsAbstract));

                viewModelTypes.AddRange(apiSpecifications.Assembly.GetSubTypes(typeof(ViewModel))
                    .Where(type => !type.IsAbstract));
            }

            var entityTypeConfigurations = entityTypeConfigurationTypes.Select(type =>
                (ITypeConfiguration) Activator.CreateInstance(type)).ToList();

            var entityViewModelMaps = entityViewModelMapTypes.Select(type =>
                    new Tuple<Type, IEntityViewModelMap>(type, (IEntityViewModelMap) Activator.CreateInstance(type)))
                .ToList();

            foreach (var entityType in entityTypes)
            {
                var config = new EntityTypePairConfiguration(entityType);
                configs.Add(config);

                if (!entityType.GetCustomAttributes<NotStoredAttribute>().Any())
                {
                    config.EntityTypeConfigurationType =
                        entityTypeConfigurations
                            .FirstOrDefault(entityConfig => entityConfig.GetEntityType() == entityType)
                            ?.GetType() ?? typeof(EntityTypeConfiguration<>).MakeGenericType(config.EntityType);
                }

                foreach (var amwAttr in entityType.GetCustomAttributes<AutoMapsWithAttribute>())
                {
                    config.ViewModelPairTypes.Add(new ViewModelMapPair(amwAttr.TargetType, null));
                    CheckIfViewModelHasMoreBindingsThan(amwAttr.TargetType, entityType, configs);
                    viewModelTypes.Remove(amwAttr.TargetType);
                }
            }

            foreach (var evmmmt in entityViewModelMaps)
            {
                var config = configs.FirstOrDefault(c => c.EntityType == evmmmt.Item2.GetEntityType());
                if (config != null)
                {
                    config.ViewModelPairTypes.Add(evmmmt.Item2.GetViewModelType(), evmmmt.Item1);
//                    Console.WriteLine("added missing viewmodel by map: " + evmmmt.Item2.GetViewModelType().Name +
//                                      " -> " + evmmmt.Item1.Name);
                }
            }

            foreach (var config in configs)
            {
                var vmName = config.EntityType.Name.Replace("Entity", "") + "ViewModel";
                var nameMatchingViewModelType = viewModelTypes.FirstOrDefault(vmType => vmType.Name == vmName);
                if (nameMatchingViewModelType != null &&
                    config.ViewModelPairTypes.All(vmpt => vmpt.ViewModelType != nameMatchingViewModelType))
                {
                    config.ViewModelPairTypes.Add(nameMatchingViewModelType, null);

//                    Console.WriteLine("added missing viewmodel by name matching: " + nameMatchingViewModelType.Name);
                }
            }

            FixEntityPairConfigurations(configs);

            Console.WriteLine("Found " + configs.Count + " entities.");

//            Log(configs);
//            Process.GetCurrentProcess().Kill();
            return configs;
        }

        private void CheckIfViewModelHasMoreBindingsThan(Type viewModelType, Type entityType,
            List<EntityTypePairConfiguration> configs)
        {
            var remainingAttrs = viewModelType.GetCustomAttributes<AutoMapsWithAttribute>()
                .Where(a => a.TargetType != entityType).ToList();
            if (remainingAttrs.Any())
            {
                foreach (var amwa in remainingAttrs)
                {
                    var config = configs.First(c => c.EntityType == amwa.TargetType);
                    config.ViewModelPairTypes.Add(new ViewModelMapPair(viewModelType, null));
                }
            }
        }

        private void FixEntityPairConfigurations(IList<EntityTypePairConfiguration> configs)
        {
            foreach (var config in configs)
            {
                foreach (var configViewModelPairType in config.ViewModelPairTypes)
                {
                    if (configViewModelPairType.ViewModelType != null &&
                        configViewModelPairType.EntityViewModelMapType == null)
                    {
                        configViewModelPairType.EntityViewModelMapType =
                            typeof(EntityViewModelMap<,>).MakeGenericType(config.EntityType,
                                configViewModelPairType.ViewModelType);
                    }
                }
            }
        }

        private void Log(IList<EntityTypePairConfiguration> configs)
        {
            var c = 0;

            Console.WriteLine("no entityConfig: " +
                              configs.Count(config => config.EntityTypeConfigurationType == null));

            foreach (var etpc in configs.OrderBy(config => config.EntityType.Name))
            {
//                if (c > 10) break;
                c++;
                Console.WriteLine($"\n#{c} ----");
                Console.WriteLine("Entity: " + etpc.EntityType?.Name);
                Console.WriteLine("EntityTypeConfig: " + etpc.EntityTypeConfigurationType?.Name);
//                Console.WriteLine("IsGenericType="+etpc.EntityTypeConfigurationType?.IsGenericType);
//                Console.WriteLine("IsConstructedGenericType="+etpc.EntityTypeConfigurationType?.IsConstructedGenericType);
//                Console.WriteLine("IsGenericTypeDefinition="+etpc.EntityTypeConfigurationType?.IsGenericTypeDefinition);
                Console.WriteLine("ViewModelPairTypes: " + etpc.ViewModelPairTypes);
            }
        }
    }
}
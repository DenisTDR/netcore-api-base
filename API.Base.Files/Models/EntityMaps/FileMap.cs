using System.IO;
using API.Base.Files.Models.Entities;
using API.Base.Files.Models.ViewModels;
using API.Base.Web.Base.Models.EntityMaps;
using AutoMapper;

namespace API.Base.Files.Models.EntityMaps
{
    public class FileMap : EntityViewModelMap<FileEntity, FileViewModel>
    {
        public override void ConfigureEntityToViewModelMapper(IMapperConfigurationExpression configurationExpression)
        {
            base.ConfigureEntityToViewModelMapper(configurationExpression);
            EntityToViewModelExpression.AfterMap((e, vm) =>
            {
                if (string.IsNullOrEmpty(e.SubDirectory) || string.IsNullOrEmpty(e.Name) ||
                    string.IsNullOrEmpty(e.Extension))
                {
                    return;
                }

                vm.Link = Path.Combine("/content", e.SubDirectory, e.Name + "." + e.Extension).Replace("\\", "/");
            });
        }
    }
}
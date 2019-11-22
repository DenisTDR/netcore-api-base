using API.Base.Files.Models.ViewModels;
using API.Base.Web.Base.Models.EntityMaps;
using AutoMapper;

namespace API.Base.Web.Common.OgMetadata
{
    public class OgMetadataEVmMap : EntityViewModelMap<OgMetadataEntity, OgMetadataViewModel>
    {
        public override void ConfigureEntityToViewModelMapper(IMapperConfigurationExpression configurationExpression)
        {
            base.ConfigureEntityToViewModelMapper(configurationExpression);
            EntityToViewModelExpression
                .ForMember(t => t.Id, opt => opt.Ignore())
                .ForMember(t => t.Image, opt => opt.Ignore())
                .AfterMap((e, vm) =>
                {
                    if (e.Image == null)
                    {
                        return;
                    }

                    vm.Image = Mapper.Map<FileViewModel>(e.Image)?.Link;
                });
        }
    }
}
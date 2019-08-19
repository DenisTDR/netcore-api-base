using System;

namespace API.Base.Web.Base.Misc
{
    public class ViewModelMapPair
    {
        public ViewModelMapPair()
        {
        }

        public ViewModelMapPair(Type viewModelType, Type entityViewModelMapType)
        {
            ViewModelType = viewModelType;
            EntityViewModelMapType = entityViewModelMapType;
        }

        public Type ViewModelType { get; set; }
        public Type EntityViewModelMapType { get; set; }

        public override string ToString()
        {
            return "{" + (ViewModelType?.Name ?? "null") + ":" + (EntityViewModelMapType?.Name ?? "null") + "}";
        }
    }
}
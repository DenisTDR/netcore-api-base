using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Base.Web.Base.Misc
{
    public class ViewModelMapPairSet : HashSet<ViewModelMapPair>
    {
        public new void Add(ViewModelMapPair pair)
        {
            var existing = this.FirstOrDefault(p =>
                pair.ViewModelType != null && p.ViewModelType == pair.ViewModelType ||
                pair.EntityViewModelMapType != null && p.EntityViewModelMapType == pair.EntityViewModelMapType);

            if (existing != null)
            {
                existing.ViewModelType = existing.ViewModelType ?? pair.ViewModelType;
                existing.EntityViewModelMapType = existing.EntityViewModelMapType ?? pair.EntityViewModelMapType;
                return;
            }

            base.Add(pair);
        }

        public void Add(Type viewModelType, Type entityViewModelMapType)
        {
            Add(new ViewModelMapPair(viewModelType, entityViewModelMapType));
        }

        public override string ToString()
        {
            return "[" + string.Join(", ", this.Select(e => e.ToString())) + "]";
        }
    }
}
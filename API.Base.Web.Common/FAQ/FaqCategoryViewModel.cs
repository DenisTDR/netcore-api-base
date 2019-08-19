using System.Collections.Generic;

namespace API.Base.Web.Common.FAQ
{
    public class FaqCategoryViewModel
    {
        public string Name { get; set; }
        public List<FaqViewModel> Faqs { get; set; }
    }
}
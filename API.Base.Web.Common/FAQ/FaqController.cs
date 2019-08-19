using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Base.Web.Base.Controllers.Api;
using API.Base.Web.Base.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Base.Web.Common.FAQ
{
    [AllowAnonymous]
    public class FaqController : GenericReadOnlyController<FaqEntity, FaqViewModel>
    {
        private readonly ILightTranslationsRepository _transRepo;

        public FaqController(ILightTranslationsRepository transRepo)
        {
            _transRepo = transRepo;
        }
        public override async Task<IActionResult> GetAll()
        {
            var allE = await Repo.GetAll();
            var allVm = Map(allE).ToList();
            foreach (var vm in allVm)
            {
                vm.Category = !string.IsNullOrEmpty(vm.Category)
                    ? await _transRepo.Translate(vm.Category)
                    : vm.Category;
            }

            var categories = new List<FaqCategoryViewModel>();
            
            foreach (var faqViewModel in allVm)
            {
                var cat = categories.FirstOrDefault(c => c.Name == faqViewModel.Category);
                if (cat == null)
                {
                    cat = new FaqCategoryViewModel {Name = faqViewModel.Category, Faqs = new List<FaqViewModel>()};
                    categories.Add(cat);
                }
                cat.Faqs.Add(faqViewModel);
            }
            

            return Ok(categories);
        } 
    }
}
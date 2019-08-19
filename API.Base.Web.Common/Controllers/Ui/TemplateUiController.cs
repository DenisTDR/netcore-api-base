using API.Base.Web.Common.Controllers.Ui.Nv;
using API.Base.Web.Common.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace API.Base.Web.Common.Controllers.Ui
{
    [Authorize(Roles = "Moderator")]
    public class TemplateUiController : NvGenericUiController<TemplateEntity>
    {
    }
}
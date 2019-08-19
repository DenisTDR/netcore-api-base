using API.Base.Web.Common.Controllers.Ui.Nv;
using Microsoft.AspNetCore.Authorization;

namespace API.Base.Web.Common.FAQ
{
    [Authorize(Roles = "Moderator")]
    public class FaqUiController : NvGenericUiController<FaqEntity>
    {
    }
}
namespace API.Base.Web.Base.Areas.Identity.Pages.Account
{
    public class StatusMessageWithType
    {
        public StatusMessageWithType()
        {
        }

        public StatusMessageWithType(string message, string type = "success")
        {
            Message = message;
            Type = type;
        }

        public string Message { get; set; }
        public string Type { get; set; }
    }
}
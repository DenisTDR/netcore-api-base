namespace API.Base.Web.Base.Models
{
    public class StatusMessageWithType
    {
        public StatusMessageWithType()
        {
        }

        public StatusMessageWithType(string message, string body = "", string type = "success")
        {
            Message = message;
            Type = type;
            Body = body;
        }

        public string Message { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }

        public StatusMessageWithType WithType(string type)
        {
            Type = type;
            return this;
        }
    }
}
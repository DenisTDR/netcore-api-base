namespace API.Base.Web.Base.Swagger
{
    public class ValidationModel
    {
        public ValidationModel()
        {
        }

        public ValidationModel(string name, object args = null, string message = null)
        {
            Name = name;
            Message = message;
            Args = args;
        }

        public string Name { get; set; }
        public string Message { get; set; }
        public object Args { get; set; }
    }
}
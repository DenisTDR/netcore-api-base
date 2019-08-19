namespace API.Base.Web.Base.Models
{
    public class ControllerActionLinkModel
    {
        public ControllerActionLinkModel()
        {
        }

        public ControllerActionLinkModel(string text, string url)
        {
            Text = text;
            Url = url;
        }

        public ControllerActionLinkModel(string text, string controller, string action = "Index")
        {
            if (!string.IsNullOrEmpty(controller))
            {
                controller = controller.Replace("Controller", "");
            }

            Text = text;
            Controller = controller;
            Action = action;
        }

        public string Text { get; set; }
        public string Url { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; } = "Index";

        public bool IsAspUrl => !string.IsNullOrEmpty(Controller);
    }
}
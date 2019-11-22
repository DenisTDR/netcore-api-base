using System;
using System.Linq;
using System.Reflection;

namespace API.Base.Web.Base.Models
{
    public class AdminDashboardLink
    {
        public string Text { get; set; }
        public string Url { get; set; }

        public Type Controller { get; set; }
        public MethodInfo Action { get; set; }

        public string ControllerName => Controller != null ? Controller.Name.Replace("Controller", "") : null;
        public string ActionName => Action != null ? Action.Name : "Index";

        public bool IsAspUrl => Controller != null && Action != null;

        public AdminDashboardLink()
        {
        }

        public AdminDashboardLink(string text, string url)
        {
            Text = text;
            Url = url;
        }

        public AdminDashboardLink(string text, Type controller, MethodInfo action = null)
        {
            if (controller != null)
            {
                if (action == null)
                {
                    action = controller.GetMethods().FirstOrDefault(mi => mi.Name == "Index");
                }
            }

            Text = text;
            Controller = controller;
            Action = action;
        }

        public AdminDashboardLink(string text, Type controller, string actionName)
            : this(text, controller, controller.GetMethods().FirstOrDefault(mi => mi.Name == actionName))
        {
        }

        public AdminDashboardLink WithMethod(string actionName)
        {
            if (Controller != null)
            {
                Action = Controller.GetMethod(actionName);
            }

            return this;
        }

        public AdminDashboardLink WithMethod(MethodInfo action)
        {
            Action = action;
            return this;
        }
    }
}
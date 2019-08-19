using System;
using System.Collections.Generic;

namespace API.Base.Web.Base.Models
{
    public class AdminDashboardConfig
    {
        public List<AdminDashboardSection> Sections { get; set; } = new List<AdminDashboardSection>();
    }

    public class AdminDashboardSection
    {
        public AdminDashboardSection()
        {
        }

        public AdminDashboardSection(string name, List<AdminDashboardLink> links)
        {
            Name = name;
            Links = links;
        }

        public string Name { get; set; }
        public List<AdminDashboardLink> Links { get; set; }
    }

    public class AdminDashboardLink : ControllerActionLinkModel
    {
        public AdminDashboardLink()
        {
        }

        public AdminDashboardLink(string text, string url) : base(text, url)
        {
        }

        public AdminDashboardLink(string text, string controller, string action = "Index") : base(text, controller,
            action)
        {
        }
    }
}
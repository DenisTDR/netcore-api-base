using System.Collections.Generic;

namespace API.Base.Web.Common.Models
{
    public class SettingsCategories
    {
        private IList<string> _list = new List<string>();
        public IEnumerable<string> List => _list;

        public void Add(string category)
        {
            _list.Add(category);
        }
    }
}
using API.Base.Web.Base.Attributes.Base;

namespace API.Base.Web.Base.Attributes
{
    public class IsReadOnlyAttribute : BasicAttribute
    {
        public bool Is => (bool) base.Value;

        public IsReadOnlyAttribute(bool value = true) : base("readOnly", value)
        {
        }
    }
}
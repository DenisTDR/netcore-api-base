namespace API.Base.Web.Base.Attributes.GenericForm
{
    public class FieldPlaceholderAttribute : FieldBasicAttribute
    {
        public FieldPlaceholderAttribute(string value) : base("placeholder", value)
        {
        }
    }
}
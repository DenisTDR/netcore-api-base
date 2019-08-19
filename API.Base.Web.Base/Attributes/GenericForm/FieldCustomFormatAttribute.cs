namespace API.Base.Web.Base.Attributes.GenericForm
{
    public class FieldCustomFormatAttribute : FieldBasicAttribute
    {
        public FieldCustomFormatAttribute(object value) : base("customFormat", value)
        {
        }
    }
}
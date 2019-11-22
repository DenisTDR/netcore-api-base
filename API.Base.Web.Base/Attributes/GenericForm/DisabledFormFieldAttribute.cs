namespace API.Base.Web.Base.Attributes.GenericForm
{
    public class DisabledFormFieldAttribute : FieldBasicAttribute
    {
        public bool Disabled => (bool) Value;
        public DisabledFormFieldAttribute(bool value = true) : base("disabled", value)
        {
        }
    }
}
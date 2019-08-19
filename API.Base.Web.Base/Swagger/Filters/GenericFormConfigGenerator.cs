using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using API.Base.Web.Base.Attributes;
using API.Base.Web.Base.Attributes.Base;
using API.Base.Web.Base.Attributes.GenericForm;
using API.Base.Web.Base.Auth.Models.HttpTransport;
using API.Base.Web.Base.Extensions;
using API.Base.Web.Base.Models.ViewModels;
using Microsoft.EntityFrameworkCore.Internal;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Base.Web.Base.Swagger.Filters
{
    public class GenericFormConfigGenerator : ISchemaFilter
    {
        public void Apply(Schema schema, SchemaFilterContext context)
        {
            if (context.SystemType.IsSubclassOf(typeof(HttpTransportBaseType)) ||
                context.SystemType.IsSubclassOf(typeof(ViewModel)))
            {
                foreach (var kvp in schema.Properties)
                {
                    TakeCareOf(kvp.Key, kvp.Value, context.SystemType);
                }
            }
        }

        private void TakeCareOf(string propertyName, Schema schema, Type declaringType)
        {
            var propertyInfo = declaringType.GetProperties()
                .FirstOrDefault(prop =>
                    string.Equals(prop.Name, propertyName, StringComparison.CurrentCultureIgnoreCase));

            if (propertyInfo == null)
            {
                return;
            }

            if (!propertyInfo.GetCustomAttributes().Any(attr => attr is IsReadOnlyAttribute))
            {
                schema.Extensions["usedInGenericForm"] = true;
            }

            PatchCustomFormats(propertyInfo, schema);
            PatchAutoSetLogic(propertyInfo, schema);

            var dataTypeAttributes = propertyInfo.GetCustomAttributes<DataTypeAttribute>();
            var validators = new List<ValidationModel>();
            foreach (var dataTypeAttribute in dataTypeAttributes)
            {
                switch (dataTypeAttribute.DataType)
                {
                    case DataType.EmailAddress:
                        schema.Extensions["format"] = "email";
                        validators.Add(new ValidationModel("email", null, "invalid_email"));
                        break;
                    case DataType.PhoneNumber:
                        schema.Extensions["format"] = "tel";
                        validators.Add(
                            new ValidationModel("pattern", "^\\+?(?:[0-9]\\s*){8,}$", "invalid_phone_number"));
                        break;
                    case DataType.Date:
                        schema.Extensions["customFormat"] = "datePicker";
                        break;
                    case DataType.MultilineText:
                        schema.Extensions["customFormat"] = "textarea";
                        break;
                }
            }

            var basicAttributes = propertyInfo.GetCustomAttributes<BasicAttribute>();
            foreach (var basicAttribute in basicAttributes)
            {
                schema.Extensions[basicAttribute.Name] = basicAttribute.Value;
            }

            PatchDefaultTexts(propertyInfo, schema);
            PatchValidation(propertyInfo, schema, validators);
        }

        private void PatchValidation(PropertyInfo propertyInfo, Schema schema, List<ValidationModel> validators)
        {
            var validationSchemaFields = new[] {"MinLength", "MaxLength", "Minimum", "Maximum"};
            foreach (var validationSchemaField in validationSchemaFields)
            {
                var propInfo = schema.GetType().GetProperty(validationSchemaField);
                var value = propInfo.GetValue(schema);
                if (value == null) continue;
                var vsf = validationSchemaField.Replace("imum", "");
                var message = "validation_" + validationSchemaField.ToSnakeCase().ToLower() + "_args";
                validators.Add(new ValidationModel(vsf.ToCamelCase(), value, message));
            }

            if (propertyInfo.GetCustomAttributes<RequiredAttribute>().Any())
            {
                validators.Add(new ValidationModel("required", null, "error-required"));
            }

            if (propertyInfo.GetCustomAttributes<RegularExpressionAttribute>().FirstOrDefault() is
                RegularExpressionAttribute regExpAttr)
            {
                validators.Add(new ValidationModel("pattern", regExpAttr.Pattern.Replace("(?i)", ""),
                    regExpAttr.ErrorMessage ?? "error-pattern"));
            }

            schema.Extensions["validators"] = validators;
        }

        private void PatchCustomFormats(PropertyInfo propertyInfo, Schema schema)
        {
            PatchEnumProperties(propertyInfo, schema);
            if (propertyInfo.PropertyType == typeof(int))
            {
                schema.Extensions["customFormat"] = "number";
            }
            else if (propertyInfo.PropertyType == typeof(bool))
            {
                schema.Extensions["customFormat"] = "checkbox";
            }
        }

        private void PatchEnumProperties(PropertyInfo propertyInfo, Schema schema)
        {
            if (!propertyInfo.PropertyType.IsEnum)
            {
                return;
            }

            var x = new List<Enum>(Enum.GetValues(propertyInfo.PropertyType).Cast<Enum>())
                .Where(enumValue =>
                    enumValue.ToString() != "None" && !propertyInfo.PropertyType.IsDefaultValue(enumValue))
                .Select(
                    enumValue => new
                    {
                        Value = enumValue.ToString(),
                        Name = enumValue.GetDisplayName(),
                        Description = enumValue.GetDisplayDescription()
                    });

            schema.Extensions["options"] = x;
            schema.Extensions["customFormat"] = "select";
        }

        private void PatchDefaultTexts(PropertyInfo propertyInfo, Schema schema)
        {
            if (!propertyInfo.GetCustomAttributes<FieldDefaultTextsAttribute>().Any())
            {
                return;
            }

            if (!schema.Extensions.ContainsKey("hint"))
            {
                schema.Extensions["hint"] = propertyInfo.Name.ToKebabCase() + "-hint";
            }

            if (!schema.Extensions.ContainsKey("placeholder"))
            {
                schema.Extensions["placeholder"] = propertyInfo.Name.ToKebabCase() + "-placeholder";
            }
        }

        private void PatchAutoSetLogic(PropertyInfo propertyInfo, Schema schema)
        {
            var autoSetAttrs = propertyInfo.GetCustomAttributes<FieldAutoSetFieldAttribute>().ToList();
            if (!autoSetAttrs.Any())
            {
                return;
            }

            schema.Extensions["autoSet"] = autoSetAttrs.Select(attr => new {from = attr.From, to = attr.To});
        }
    }
}
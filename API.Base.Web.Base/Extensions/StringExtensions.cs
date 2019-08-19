using System.Linq;
using System.Text;

namespace API.Base.Web.Base.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string source, bool toPascal = false)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            if (!toPascal)
            {
                if (char.IsUpper(source[0]))
                {
                    source = char.ToLower(source[0]) + source.Substring(1);
                }
            }
            else
            {
                if (char.IsLower(source[0]))
                {
                    source = char.ToUpper(source[0]) + source.Substring(1);
                }
            }

            if (!source.Contains("_"))
            {
                return source;
            }

            var sb = new StringBuilder();
            for (var i = 0; i < source.Length; i++)
            {
                var c = source[i];

                if (c == '_')
                {
                    i++;
                    if (i < source.Length)
                    {
                        sb.Append(char.ToUpper(source[i]));
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static string ToSnakeCase(this string source)
        {
            return string.Concat(source.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()))
                .ToLower();
        }

        public static string ToKebabCase(this string source)
        {
            return string.Concat(source.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x.ToString() : x.ToString()))
                .ToLower();
        }
    }
}
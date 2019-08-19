using System;

namespace API.Base.Web.Base.Misc
{
    public static class Utilis
    {
        private const int SelectorLength = 32;
        public static string GenerateRandomHexString(int length = 20)
        {
            var str = "";
            while (str.Length < length)
            {
                str += Guid.NewGuid().ToString().ToLower().Replace("-", "");
            }
            return str.Substring(0, length);
        }

        public static string GenerateSelector()
        {
            return GenerateRandomHexString(SelectorLength);
        }

        public static bool IsSelector(string str)
        {
            return !string.IsNullOrEmpty(str) && str.Length == SelectorLength;
        }
    }
}

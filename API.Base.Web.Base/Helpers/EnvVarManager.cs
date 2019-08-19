using System;

namespace API.Base.Web.Base.Helpers
{
    public static class EnvVarManager
    {
        public static string GetOrThrow(string name)
        {
            var value = Get(name);
            if (string.IsNullOrEmpty(value))
            {
                var str = "Env var '" + name + "' is not set";
                Console.WriteLine(str);
                throw new Exception(str);
            }
            return value;
        }

        public static string Get(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);
            return value;
        }
    }
}
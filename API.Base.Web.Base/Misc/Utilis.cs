using System;
using System.Diagnostics;

namespace API.Base.Web.Base.Misc
{
    public static class Utilis
    {
        public static string GenerateRandomHexString(int length = 20)
        {
            var str = "";
            while (str.Length < length)
            {
                str += Guid.NewGuid().ToString().ToLower().Replace("-", "");
            }

            return str.Substring(0, length);
        }

        public static void DieWith(string message, bool killProcess = true)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
            if (killProcess)
            {
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}
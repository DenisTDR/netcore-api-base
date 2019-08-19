using System;

namespace API.Base.Web.Base.Exceptions
{
    public class KnownException : Exception
    {
        public int Code { get; private set; }

        public KnownException(string message, int code = 400, Exception innerException = null) : base(message, innerException)
        {
            Code = code;
        }
    }
}
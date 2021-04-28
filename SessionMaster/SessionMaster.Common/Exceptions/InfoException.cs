using System;
using System.Globalization;

namespace SessionMaster.Common.Exceptions
{
    public class InfoException : Exception
    {
        public InfoException() : base()
        {
        }

        public InfoException(string message) : base(message)
        {
        }

        public InfoException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
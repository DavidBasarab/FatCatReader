using System;

namespace GoogleAPIRequestTester
{
    public static class Extensions
    {
        public static string FormatWithNewLine(this string message, object[] args)
        {
            return String.Format("{0}{1}", String.Format(message, args), Environment.NewLine);
        }
    }
}
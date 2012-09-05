using System;

namespace GoogleAPIRequestTester.Models
{
    public class ReaderMethod
    {
        public ReaderMethod(string methodName, Action method)
        {
            Name = methodName;
            Method = method;
        }

        public string Name { get; set; }
        public Action Method { get; set; }
    }
}
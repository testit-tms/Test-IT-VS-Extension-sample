using System;

namespace TestIT.Tests.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ExportAutoTestAttribute : Attribute
    {
        public string Identifier { get; }

        public ExportAutoTestAttribute(string identifier)
        {
            Identifier = identifier;
        }
    }
}

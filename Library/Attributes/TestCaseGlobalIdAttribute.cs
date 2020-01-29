using System;

namespace TestIT.Linker.Attributes
{
    /// <summary>
    /// Attribute to define Test It test cases to link with curent Test method. Required for linking
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestCaseGlobalIdAttribute : Attribute
    {
        public long[] GlobalIds { get; }

        public TestCaseGlobalIdAttribute(params long[] globalIds)
        {
            GlobalIds = globalIds;
        }
    }
}

namespace TestIT.Linker.Models.Struct
{
    public struct TestToAutotestConnection
    {

        public TestToAutotestConnection(long testCaseExternalID, string autotestExternalId)
        {
            TestCaseGlobalId = testCaseExternalID;
            AutotestExternalId = autotestExternalId;
        }

        public long TestCaseGlobalId { get; set; }

        public string AutotestExternalId { get; set; }
    }
}

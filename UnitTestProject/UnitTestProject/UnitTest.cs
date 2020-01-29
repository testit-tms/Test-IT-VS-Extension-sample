using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void UnitTest1()
        {
            Step1();
            Step2();
            Step3();
        }

        [TestMethod]
        public void UnitTest2()
        {
            Step1();
            Step2();
            Step3();
            Step4();
        }
        [TestMethod]
        public void UnitTest3()
        {
            Step1();
            Step2();
            Step3();
            Step4();
            Step5();
        }

        private void Step1() { }
        private void Step2() { }
        private void Step3() { }
        private void Step4() { }
        private void Step5() { }
    }
}

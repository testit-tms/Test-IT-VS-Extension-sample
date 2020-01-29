using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject
{
    [TestClass]
    public class IntegrationTests
    {
        [TestMethod]
        public void IntegrationTest1()
        {
            Step1();
            Step2();
            Step3();
        }

        [TestMethod]
        public void IntegrationTest2()
        {
            Step1();
            Step2();
            Step3();
            Step4();
        }
        [TestMethod]
        public void IntegrationTest3()
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
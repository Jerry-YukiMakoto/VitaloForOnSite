using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mirle.DB.Proc;
using Mirle.Def;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;

namespace Mirle.DB.Proc.Tests
{
    [TestClass()]
    public class clsProcTests
    {
        [TestMethod()]
        public void CheckFunMethod()
        {
            clsDbConfig config=null;
            clsDbConfig config_Sqlite=null;
            //var mock = new Mock<>();
            var Test = new clsProc(config,config_Sqlite);
            int expectedvalus = 11;
            int actual=Test.StoreInfindpathbyEquNo(1);
            Assert.AreEqual(expectedvalus, actual);
            expectedvalus = 12;
            actual = Test.StoreInfindpathbyEquNo(2);
            Assert.AreEqual(expectedvalus, actual);
            expectedvalus = 13;
            actual = Test.StoreInfindpathbyEquNo(3);
            Assert.AreEqual(expectedvalus, actual);
            expectedvalus = 14;
            actual = Test.StoreInfindpathbyEquNo(4);
            Assert.AreEqual(expectedvalus, actual);
            expectedvalus = 15;
            actual = Test.StoreInfindpathbyEquNo(5);
            Assert.AreEqual(expectedvalus, actual);
            expectedvalus = 16;
            actual = Test.StoreInfindpathbyEquNo(6);
            Assert.AreEqual(expectedvalus, actual);

            var rand = new Random();
            expectedvalus = 0;
            actual = Test.StoreInfindpathbyEquNo(rand.Next(7,100000000));
            Assert.AreEqual(expectedvalus, actual);
        }

        [TestMethod()]
        public void CheckEqu()
        {
            clsDbConfig config = null;
            clsDbConfig config_Sqlite = null;
            //var mock = new Mock<>();
            var Test = new clsProc(config, config_Sqlite);
            for (int i = 0; i < 10000; i++)
            {
                int equno = Int32.Parse(clsProc.GetEquNo());

                Assert.IsTrue(equno > 0 && equno < 7, "equno out of range");

            }
        }
    }
}
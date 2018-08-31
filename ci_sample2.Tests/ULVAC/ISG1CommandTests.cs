using Microsoft.VisualStudio.TestTools.UnitTesting;
using STECCommon.Equipment.ULVAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STECCommon.Equipment.ULVAC.Tests
{
    [TestClass()]
    public class ISG1CommandTests
    {
        [TestMethod()]
        public void CalcChecksumTest()
        {
            string checkSum = ISG1Command.CalcChecksum(":11D");

            Assert.AreEqual("44", checkSum);

            //  取説の例に間違いあり、注意
            checkSum = ISG1Command.CalcChecksum(":11D4.00E05F6");

            Assert.AreEqual("6E", checkSum);

            checkSum = ISG1Command.CalcChecksum(":01n");

            Assert.AreEqual("6F", checkSum);
        }

        [TestMethod()]
        public void ISG1CommandTest()
        {
            var command = new ISG1Command(11, "D");

            Assert.AreEqual(":11D44", command.GetCommandString());
        }

        [TestMethod()]
        public void ISG1CommandTest1()
        {
            var command = new ISG1Command(":11D4.00E05F66E");

            Assert.AreEqual(11, command.Address);
            Assert.AreEqual("D4.00E05F6", command.Parameter);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ISG1CommandTest2()
        {
            //  アドレス解釈失敗
            var command = new ISG1Command(":1BD4.00E05F66E");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ISG1CommandTest3()
        {
            //  チェックサム異常
            var command = new ISG1Command(":11D4.00E05F660");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ISG1CommandTest4()
        {
            //  コマンド長不足
            var command = new ISG1Command(":01AA");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ISG1CommandTest5()
        {
            //  チェックサム異常(レスポンスのパラメータがn）
            var command = new ISG1Command(":01n6F");
        }

        [TestMethod()]
        public void FailTest()
        {
            Assert.AreEqual(10, 20, "単体テスト失敗テスト");
        }
    }
}
using System;
using System.Text.RegularExpressions;
using Melek.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MtGBar.Infrastructure.Utilities.VendorRelations;

namespace MtGBarTests
{
    [TestClass]
    public class VendorRelationsTests
    {
        [TestMethod]
        public void MtgoTradersGetLinkWorks()
        {
            string expected = "http://www.mtgotraders.com/store/AVR_Restoration_Angel.html";
            MtgoTraders vendorClient = new MtgoTraders();

            Assert.AreEqual(expected, vendorClient.GetLink(new Card() { Name = "Restoration Angel" }, new Set() { Code = "AVR" }));
        }

        [TestMethod]
        public void MtgoTradersPriceIsReasonable()
        {
            string expectedRegex = "\\$[0-9\\.]+";
            MtgoTraders vendorClient = new MtgoTraders();
            string price = vendorClient.GetPrice(new Card() { Name = "Restoration Angel" }, new Set() { Code = "AVR" });
            Assert.IsTrue(Regex.IsMatch(price, expectedRegex));
        }
    }
}
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DealTracker.Controllers;
using System.Web.Mvc;

namespace DealTrackerTests.Controllers
{
    [TestClass]
    public class ImportControllersTest
    {
        [TestMethod]
        public void Index()
        {
            //Arrange
            ImportController controller = new ImportController();

            //Act
            ViewResult result = controller.Index() as ViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "View");
            Assert.AreNotEqual("Error while parsing the CSV file", result.ViewBag.Message);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Board.Controllers;
using System.Web.Mvc;

namespace Board.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            var controller = new HomeController();
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}

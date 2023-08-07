using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using EStore.Models;
using Unity;
using EStore.Utilities.DataRepository;
using EStore.Utilities;
using EStore.Controllers;
using System.Web.Mvc;
using Moq;
using System.Web;

namespace EStoreTest
{
    [TestClass]
    public class UnitTest1
    {
        private IUnityContainer container;
        private Mock<ControllerContext> mockControllerContext;

        [TestInitialize]
        public void Setup()
        {
            UnityConfig.RegisterComponents();

            container = new UnityContainer();
            container.RegisterType<IUserDataRepository, DummyUserDataRepository>();
            container.RegisterType<IProductDataRepository, DummyProductDataRepository>();
            container.RegisterType<IDashboardDataRepository, DummyDashboardDataRepository>();
            container.RegisterType<ITotalSalesDataRepository, DummyCartDataRepository>();
            container.RegisterType<IProductDataRepositoryV2, DummyProductDataRepositoryV2>();
            mockControllerContext = new Mock<ControllerContext>();
            var mockSession = new Mock<HttpSessionStateBase>();
            mockSession.SetupGet(s => s[User.UserSessionString]).Returns(DummyData.UserModelData);
            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
        }

        private HomeController GetHomeController()
        {
            return new HomeController(
                container.Resolve<IProductDataRepository>(),
                container.Resolve<IUserDataRepository>(),
                container.Resolve<ITotalSalesDataRepository>(),
                container.Resolve<IProductDataRepositoryV2>());
        }

        [TestMethod]
        [DataRow("testUsername", "test@123", "validUser")]
        [DataRow("testAdmin", "test@123", "validAdmin")]
        [DataRow("test", "test", "inValidUser")]
        public void LoginTest(string userName, string passWord, string type)
        {
            var controller = new LoginController(container.Resolve<IUserDataRepository>());
            controller.ControllerContext = mockControllerContext.Object;

            UserLoginCredentials userCred = new UserLoginCredentials()
            {
                UserName = userName,
                Password = passWord
            };

            var result = controller.SignIn(userCred);

            Assert.IsNotNull(result);

            if(type.Equals("inValidUser"))
            {
                var res = result as ViewResultBase;
                Assert.AreEqual("", res.ViewName);
                Assert.IsNotNull(res.Model);
            }
            else if (result is RedirectResult redirectResult) 
            {
                Assert.IsTrue(result is RedirectToRouteResult);
                switch (type)
                {
                    case "validUser": Assert.AreEqual("/Home/Index", redirectResult.Url);break;
                    case "validAdmin": Assert.AreEqual("/Admin/Dashboard", redirectResult.Url);break;
                }
            }
        }

        [TestMethod]
        public void RegisterUserTest()
        {
            var controller = new LoginController(container.Resolve<IUserDataRepository>());
            controller.ControllerContext = mockControllerContext.Object;

            var result = controller.Register(DummyData.UserModelData);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is RedirectToRouteResult);

            if (result is RedirectResult redirectResult)
            {
                Assert.AreEqual("/Login/SignIn", redirectResult.Url);
            }
        }

        [TestMethod]
        public void ProductPostTest()
        {
            var controller = GetHomeController();
            controller.ControllerContext = mockControllerContext.Object;

            var result = controller.Product(DummyData.ProductModelData.Name);

            Assert.IsNotNull(result);

            if (result is RedirectResult redirectResult)
            {
                Assert.AreEqual("/Home/Index", redirectResult.Url);
            }
        }

        [TestMethod]
        public void UpdateUserPageTest()
        {
            var controller = GetHomeController();
            controller.ControllerContext = mockControllerContext.Object;

            var result = controller.UpdateUser();

            Assert.IsNotNull(result);

            if (result is RedirectResult redirectResult)
            {
                Assert.AreEqual("/Home/UpdateUser", redirectResult.Url);
            }
        }

        [TestMethod]
        public void UpdateUserPostTest()
        {
            var controller = GetHomeController();
            controller.ControllerContext = mockControllerContext.Object;

            var result = controller.UpdateUser(DummyData.UserModelData);

            Assert.IsNotNull(result);

            if (result is RedirectResult redirectResult)
            {
                Assert.AreEqual("/Home/Index", redirectResult.Url);
            }
        }
    }
}

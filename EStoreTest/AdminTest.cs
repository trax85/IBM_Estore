using EStore.Utilities.DataRepository;
using EStore.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web.Mvc;
using System.Web;
using Unity;
using EStore.Models;
using EStore.Controllers;

namespace EStoreTest
{
    [TestClass]
    public class AdminTest
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
            container.RegisterType<IContactUsDataRepository, DummyContactUsDataRepository>();
            mockControllerContext = new Mock<ControllerContext>();
            var mockSession = new Mock<HttpSessionStateBase>();
            mockSession.SetupGet(s => s[User.UserSessionString]).Returns(DummyData.AdminUserModelData);
            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
        }

        private AdminController GetAdminController()
        {
            return new AdminController(
                container.Resolve<IUserDataRepository>(),
                container.Resolve<IProductDataRepository>(),
                container.Resolve<ITotalSalesDataRepository>(),
                container.Resolve<IContactUsDataRepository>()
                );
        }

        [TestMethod]
        public void ContactInfoUpdate()
        {
            var controller = GetAdminController();
            controller.ControllerContext = mockControllerContext.Object;

            var result = controller.ContactInfo(DummyData.ContactUsModel);

            Assert.IsNotNull(result);
            if (result is RedirectResult redirectResult)
            {
                Assert.AreEqual("/Admin/Dashboard", redirectResult.Url);
            }
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateUserPost(bool isEdit)
        {
            var controller = GetAdminController();
            controller.ControllerContext = mockControllerContext.Object;

            var result = controller.Save(DummyData.UserModelData, isEdit);
            Assert.IsNotNull(result);
            if (result is RedirectResult redirectResult)
            {
                Assert.AreEqual("/Admin/Users", redirectResult.Url);
            }
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void CreateProductPost(bool isEdit)
        {
            var controller = GetAdminController();
            controller.ControllerContext = mockControllerContext.Object;

            var result = controller.SaveProduct(DummyData.ProductModelData, isEdit);
            Assert.IsNotNull(result);
            if (result is RedirectResult redirectResult)
            {
                Assert.AreEqual("/Admin/Products", redirectResult.Url);
            }
        }
    }
}

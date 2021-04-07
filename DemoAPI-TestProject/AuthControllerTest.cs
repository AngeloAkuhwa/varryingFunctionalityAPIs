using DemoAPI_app.DTOs;
using DemoAPI_app.Service;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Threading.Tasks;
using DemoAPI_app.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace DemoAPI_TestProject
{
    public class AuthControllerTest
    {
        private IServiceProvider _serviceProvider;
        private AuthController _authController;

        public static Mock<IUserStore<IdentityUser>> Store = new Mock<IUserStore<IdentityUser>>();
        public Mock<UserManager<IdentityUser>> UserManger = new Mock<UserManager<IdentityUser>>(Store.Object, null,null,null,null,null,null,null,null);
        public Mock<IUserService> UserService = new Mock<IUserService>();
        public Mock<IServiceProvider> ServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);


        [SetUp]
        public void MockIServiceProviderSetUp()
        {
            ServiceProvider.Setup(x => x.GetService(typeof(UserManager<IdentityUser>))).Returns(UserManger.Object).Verifiable();
            ServiceProvider.Setup(x =>x.GetService(typeof(IUserService))).Returns(UserService.Object).Verifiable();

            _serviceProvider = ServiceProvider.Object;
            _authController = new AuthController(_serviceProvider);
        }

        public void RegisterMockUp(bool success)
        {
            UserService.Setup(x => x.RegisterUserAsync(It.IsAny<RegisterDTO>()))
                .ReturnsAsync(new UsermanagerResponseDTO { IsSuccessful = success });
        }

        public void LoginMockUp(bool success)
        {
            UserService.Setup(x => x.LoginUserAsync(It.IsAny<LoginDTO>()))
                .ReturnsAsync(new UsermanagerResponseDTO { IsSuccessful = success });

        }

        public void ChangePasswordMockUp(bool success)
        {
            UserService.Setup(x => x.ChangeUserPasswordAsync(It.IsAny<ChangePasswordDTO>()))
                .ReturnsAsync(new UsermanagerResponseDTO() { IsSuccessful = success });

        }


        //test using InstanceOf
        [Test]
        public async Task UserRegisterTest()
        {
            //arrange
            var regDto = new RegisterDTO(){Email = "",Password = "",ConfirmPassword = ""};
            

            //act
            RegisterMockUp(false);
            var actual = await _authController.RegisterAsync(regDto) as BadRequestObjectResult;

            RegisterMockUp(true);
            var actual1 = await _authController.RegisterAsync(regDto) as CreatedResult;

            //assert
            Assert.IsInstanceOf<BadRequestObjectResult>(actual);
            Assert.IsInstanceOf<CreatedResult>(actual1);
        }


        //test using status codes with the help of ControllerContext
        [Test]
        public async Task UserLoginTest()
        {
            //arrange
            var loginDto = new LoginDTO{Email = "",Password = ""};
            _authController.ControllerContext.HttpContext = new DefaultHttpContext();
            var expected = 400;
            var expected1 = 200;

            //act
            LoginMockUp(false);
            var actual = await _authController.LoginAsync(loginDto) as BadRequestObjectResult;

            LoginMockUp(true);
            var actual1 = await _authController.LoginAsync(loginDto) as OkObjectResult;

            //assert

            Assert.AreEqual(expected, actual.StatusCode);
            Assert.AreEqual(expected1,actual1.StatusCode);
        }



        //test using InstanceOf
        [Test]
        public async Task UserChangePasswordTest()
        {
            //arrange
            var changePasswordDto = new ChangePasswordDTO { Email = "", OldPassword =  "", ConfirmPassword = "" };


            //act
            ChangePasswordMockUp(false);
            var actual = await _authController.ChangePasswordAsync(changePasswordDto) as BadRequestObjectResult;

            ChangePasswordMockUp(true);
            var actual1 = await _authController.ChangePasswordAsync(changePasswordDto) as OkObjectResult;

            //assert
            Assert.IsInstanceOf<BadRequestObjectResult>(actual);
            Assert.IsInstanceOf<OkObjectResult>(actual1);
        }
    }
}

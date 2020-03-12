using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    // getAll, getById, getByName
    [TestClass]
    public class UserControllerTests
    {
        private Mock<IUserService> _userService = new Mock<IUserService>();


        [TestMethod]
        public void UsersConrtoller_GetAsync_ReturnAllUsers()
        {
            //Arrange
            List<UserDomainModel> userDomainModelList = new List<UserDomainModel>();
            UserDomainModel userDomainModel = new UserDomainModel
            {
                Id = Guid.NewGuid(),
                FirstName = "First name",
                LastName = "Last name",
                UserName = "User name",
                IsAdmin = true
            };

            userDomainModelList.Add(userDomainModel);
            IEnumerable<UserDomainModel> userDomainModels = userDomainModelList;
            Task<IEnumerable<UserDomainModel>> responseTask = Task.FromResult(userDomainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _userService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            UsersController usersController = new UsersController(_userService.Object);

            //Act
            var result = usersController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var userDomainModelResultList = (List<UserDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(userDomainModelResultList);
            Assert.AreEqual(expectedResultCount, userDomainModelResultList.Count);
            Assert.AreEqual(userDomainModel.Id, userDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }


        [TestMethod]
        public void UsersConrtoller_GetAsync_ReturnNull()
        {
            //Arrange
            IEnumerable<UserDomainModel> userDomainModels = null;
            Task<IEnumerable<UserDomainModel>> responseTask = Task.FromResult(userDomainModels);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _userService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            UsersController usersController = new UsersController(_userService.Object);

            //Act
            var result = usersController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var userDomainModelResultList = (List<UserDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(userDomainModelResultList);
            Assert.AreEqual(expectedResultCount, userDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void UsersConrtoller_GetByIdAsync_ReturnUser()
        {

            //Arrange
            Guid guid = Guid.NewGuid();
            UserDomainModel userDomainModel = new UserDomainModel
            {
                UserName = "User name",
            };

            UserDomainModel userDomainModels = userDomainModel;
            Task<UserDomainModel> responseTask = Task.FromResult(userDomainModels);
            int expectedStatusCode = 200;

            _userService.Setup(x => x.GetUserByIdAsync(guid)).Returns(responseTask);
            UsersController usersController = new UsersController(_userService.Object);

            //Act
            var result = usersController.GetbyIdAsync(guid).ConfigureAwait(false).GetAwaiter().GetResult().Result;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }


        [TestMethod]
        public void UsersConrtoller_GetByIdAsync_ReturnUserNotFound()
        {

            //Arrange
            Guid guid = Guid.NewGuid();

            UserDomainModel userDomainModels = null;
            Task<UserDomainModel> responseTask = Task.FromResult(userDomainModels);
            int expectedStatusCode = 404;

            _userService.Setup(x => x.GetUserByIdAsync(guid)).Returns(responseTask);
            UsersController usersController = new UsersController(_userService.Object);

            //Act
            var result = usersController.GetbyIdAsync(guid).ConfigureAwait(false).GetAwaiter().GetResult().Result;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void UsersConrtoller_GetByUsernameAsync_ReturnUser()
        {

            //Arrange

            UserDomainModel userDomainModel = new UserDomainModel
            {
                UserName = "User name",
            };

            UserDomainModel userDomainModels = userDomainModel;
            Task<UserDomainModel> responseTask = Task.FromResult(userDomainModels);
            int expectedStatusCode = 200;

            _userService.Setup(x => x.GetUserByUserName(userDomainModels.UserName)).Returns(responseTask);
            UsersController usersController = new UsersController(_userService.Object);

            //Act
            var result = usersController.GetbyUserNameAsync(userDomainModels.UserName).ConfigureAwait(false).GetAwaiter().GetResult().Result;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }


        [TestMethod]
        public void UsersConrtoller_GetByUsernameAsync_ReturnUserNotFound()
        {
            //Arrange
            string name = "";

            UserDomainModel userDomainModels = null;
            Task<UserDomainModel> responseTask = Task.FromResult(userDomainModels);
            int expectedStatusCode = 404;

            _userService.Setup(x => x.GetUserByUserName(name)).Returns(responseTask);
            UsersController usersController = new UsersController(_userService.Object);

            //Act
            var result = usersController.GetbyUserNameAsync(name).ConfigureAwait(false).GetAwaiter().GetResult().Result;


            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)result).StatusCode);
        }

    }
}

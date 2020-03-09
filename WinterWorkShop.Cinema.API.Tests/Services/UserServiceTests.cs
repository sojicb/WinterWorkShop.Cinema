using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    //GetAll, GetById, GetByUserName
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IUsersRepository> _mockUsersRepository;
        private User _user;
        private UserDomainModel _userDomainModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "First name",
                LastName = "Last name",
                UserName = "User name",
                IsAdmin = false
            };

            _userDomainModel = new UserDomainModel
            {
                Id = Guid.NewGuid(),
                FirstName = "First name",
                LastName = "Last name",
                UserName = "User name",
                IsAdmin = false
            };

            List<User> userModelList = new List<User>();

            userModelList.Add(_user);
            IEnumerable<User> users = userModelList;
            Task<IEnumerable<User>> responseTask = Task.FromResult(users);

            _mockUsersRepository = new Mock<IUsersRepository>();
            _mockUsersRepository.Setup(x => x.GetAll()).Returns(responseTask);
        }

        [TestMethod]
        public void UserService_GetAllAsync_ReturnListOfUsers()
        {
            //Arrange
            int expectedResultCount = 1;
            UserService usersController = new UserService(_mockUsersRepository.Object);
            //Act
            var resultAction = usersController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<UserDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_user.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(UserDomainModel));
        }

        [TestMethod]
        public void UserService_GetAllAsync_ReturnNull()
        {
            //Arrange
            IEnumerable<User> users = null;
            Task<IEnumerable<User>> responseTask = Task.FromResult(users);

            _mockUsersRepository = new Mock<IUsersRepository>();
            _mockUsersRepository.Setup(x => x.GetAll()).Returns(responseTask);
            UserService usersController = new UserService(_mockUsersRepository.Object);

            //Act
            var resultAction = usersController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

       

    }
}

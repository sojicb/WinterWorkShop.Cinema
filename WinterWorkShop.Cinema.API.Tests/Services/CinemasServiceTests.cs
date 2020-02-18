using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class CinemasServiceTests
    {
        private Mock<ICinemasRepository> _mockCinemasRepository;
        private Data.Cinema _cinema;
        private CinemaDomainModel _cinemaDomainModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _cinema = new Data.Cinema
            {
                Id = new int(),
                Name = "NoviCinema"
            };

            _cinemaDomainModel = new CinemaDomainModel
            {
                Id = new int(),
                Name = "NoviCinema"
            };

            List<Data.Cinema> cinemasModelsList = new List<Data.Cinema>();

            cinemasModelsList.Add(_cinema);
            IEnumerable<Data.Cinema> cinemas = cinemasModelsList;
            Task<IEnumerable<Data.Cinema>> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(responseTask);
        }

        [TestMethod]
        public void CinemaService_GetAllAsync_ReturnListOfCinemas()
        {
            //Arrange
            int expectedResultCount = 1;
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object);

            //Act
            var resultAction = cinemasController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<ProjectionDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_cinema.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(ProjectionDomainModel));
        }

        [TestMethod]
        public void CinemaService_GetAllAsync_ReturnNull()
        {
            //Arrange
            IEnumerable<Data.Cinema> cinemas = null;
            Task<IEnumerable<Data.Cinema>> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(responseTask);
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object);

            //Act
            var resultAction = cinemasController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void CinemaService_CreateCinema_InsertMockedNull_ReturnErrorMessage()
        {
            //Arrange
            List<Data.Cinema> cinemasModelsList = new List<Data.Cinema>();
            _cinema = null;
            string expectedMessage = "Error occured while creating new projection, please try again.";

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.Equals(It.IsAny<int>())).Returns(cinemasModelsList);
            _mockCinemasRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Returns(_cinema);
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object);

            //Act
            var resultAction = cinemasController
                .CreateCinema(_cinemaDomainModel)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class CinemaServiceTests
    {
        private Mock<ICinemasRepository> _mockCinemasRepository;
        private WinterWorkShop.Cinema.Data.Cinema _cinema;
        private CinemaDomainModel _cinemaDomainModel;

        [TestInitialize]
        public void TestInitialize()
        {

            _cinema = new Data.Cinema
            {
                Id = 1,
                Name = "Cinema name",

            };

            _cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "Cinema name"
            };

            List<WinterWorkShop.Cinema.Data.Cinema> cinemasModelsList = new List<WinterWorkShop.Cinema.Data.Cinema>();
            
            cinemasModelsList.Add(_cinema);
            IEnumerable<WinterWorkShop.Cinema.Data.Cinema> cinemas = cinemasModelsList;
            Task<IEnumerable<WinterWorkShop.Cinema.Data.Cinema>> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(responseTask);
        }



        [TestMethod]
        public void CinemaService_GetAllAsync_ReturnListOfCinemas()
        {
            //Arrange
            int expectedResultCount = 1;
            CinemaService cinemaService = new CinemaService(_mockCinemasRepository.Object);
            //Act
            var resultAction = cinemaService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<CinemaDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_cinema.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(CinemaDomainModel));
        }

        [TestMethod]
        public void CinemaService_GetAllAsync_ReturnNull()
        {
            //Arrange
            IEnumerable<WinterWorkShop.Cinema.Data.Cinema> cinemas = null;
            Task<IEnumerable<WinterWorkShop.Cinema.Data.Cinema>> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(responseTask);
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object);

            //Act
            var resultAction = cinemasController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }


    }
}

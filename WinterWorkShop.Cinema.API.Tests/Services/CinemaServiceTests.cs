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
    [TestClass]
    public class CinemaServiceTests
    {
        private Mock<ICinemasRepository> _mockCinemasRepository;
        private Mock<IAuditoriumsRepository> _mockAduitoriumsRepository;
        private Mock<IAuditoriumService> _mockAuditoriumService;
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
            List<Auditorium> auditoriumModelsList = new List<Auditorium>();
            List<AuditoriumService> auditoriumServicesList = new List<AuditoriumService>();

            cinemasModelsList.Add(_cinema);
            IEnumerable<WinterWorkShop.Cinema.Data.Cinema> cinemas = cinemasModelsList;
            Task<IEnumerable<WinterWorkShop.Cinema.Data.Cinema>> responseTask = Task.FromResult(cinemas);
            IEnumerable<Auditorium> auditoriums = auditoriumModelsList;
            IEnumerable<AuditoriumService> auditoriumServices = auditoriumServicesList;

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(responseTask);
            _mockAduitoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumService = new Mock<IAuditoriumService>();
        }



        [TestMethod]
        public void CinemaService_GetAllAsync_ReturnListOfCinemas()
        {
            //Arrange
            int expectedResultCount = 1;
            CinemaService cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAduitoriumsRepository.Object, _mockAuditoriumService.Object);
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
       public void CinemaService_GetCinemaById_ReturnCinema()
        {
            //Arrange
            IEnumerable<WinterWorkShop.Cinema.Data.Cinema> cinemas = null;
            Task<IEnumerable<WinterWorkShop.Cinema.Data.Cinema>> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<WinterWorkShop.Cinema.Data.Cinema>()));
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object, _mockAduitoriumsRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = cinemasController.GetCinemaByIdAsync(responseTask.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void CinemaService_GetAllAsync_ReturnNull()
        {
            //Arrange
            IEnumerable<WinterWorkShop.Cinema.Data.Cinema> cinemas = null;
            Task<IEnumerable<WinterWorkShop.Cinema.Data.Cinema>> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(responseTask);
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object, _mockAduitoriumsRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = cinemasController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }


        [TestMethod]
        public void CinemaService_DeleteCinema_ReturnNull()
        {
            //Arrange
            IEnumerable<WinterWorkShop.Cinema.Data.Cinema> cinemas = null;
            Task<IEnumerable<WinterWorkShop.Cinema.Data.Cinema>> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.Delete(It.IsAny<WinterWorkShop.Cinema.Data.Cinema>()));
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object, _mockAduitoriumsRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = cinemasController.DeleteCinema(responseTask.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);

        }

        [TestMethod]
        public void CinemaService_UpdateCinema_Cinema()
        {
            List<WinterWorkShop.Cinema.Data.Cinema> cinemaModelsList = new List<WinterWorkShop.Cinema.Data.Cinema>();

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()));
            _mockCinemasRepository.Setup(x => x.Update(It.IsAny<WinterWorkShop.Cinema.Data.Cinema>())).Returns(_cinema);
            _mockCinemasRepository.Setup(x => x.Save());
            CinemaService cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAduitoriumsRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = cinemaService.UpdateCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);

        }



        [TestMethod]
        public void CinemaService_CreationCinema_InsertMockedNull_ReturnErrorMessage()
        {
            
            List<WinterWorkShop.Cinema.Data.Cinema> cinemaModelsList = new List<WinterWorkShop.Cinema.Data.Cinema>();
            _cinema = null;
            string expectedMessage = "Error occured while creating a new cinema, please try again.";

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<WinterWorkShop.Cinema.Data.Cinema>()));
            _mockCinemasRepository.Setup(x => x.Insert(It.IsAny<WinterWorkShop.Cinema.Data.Cinema>())).Returns(_cinema);
            CinemaService cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAduitoriumsRepository.Object, _mockAuditoriumService.Object);

            var resultAction = cinemaService.AddCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }


        [TestMethod]
        public void CinemaService_CreateCinema_InsertMocked_ReturnCinema()
        {
            //Arrange
            List<WinterWorkShop.Cinema.Data.Cinema> cinemaModelsList = new List<WinterWorkShop.Cinema.Data.Cinema>();

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()));
            _mockCinemasRepository.Setup(x => x.Insert(It.IsAny<WinterWorkShop.Cinema.Data.Cinema>())).Returns(_cinema);
            _mockCinemasRepository.Setup(x => x.Save());
            CinemaService cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAduitoriumsRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = cinemaService.AddCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_cinema.Id, resultAction.Cinema.Id);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
        }



    }
}

﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private Mock<IAuditoriumService> _mockAuditoriumService;
        private Data.Cinema _cinema;
        private Auditorium _auditorium;
        private CinemaDomainModel _cinemaDomainModel;
        private DeleteAuditoriumDomainModel _auditoriumDomainModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _auditoriumDomainModel = new DeleteAuditoriumDomainModel
            {
                IsSuccessful = true,
                ErrorMessage = null
             };

            _auditorium = new Auditorium
            {
                Id = 1,
                CinemaId = 1
            };
            
            _cinema = new Data.Cinema
            {
                Id = 1,
                Name = "Cinema name",
                Auditoriums = new List<Auditorium>
                {
                    _auditorium
                }
            };

            _cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "Cinema name"
            };
        }



        [TestMethod]
        public void CinemaService_GetAllAsync_ReturnListOfCinemas()
        {
            //Arrange
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            List<Data.Cinema> cinemasModelsList = new List<Data.Cinema>();

            cinemasModelsList.Add(_cinema);
            IEnumerable<Data.Cinema> cinemas = cinemasModelsList;
            Task<IEnumerable<Data.Cinema>> responseTask = Task.FromResult(cinemas);
            int expectedResultCount = 1;
            CinemaService cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);

            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(responseTask);
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
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            IEnumerable<Data.Cinema> cinemas = null;
            Task<IEnumerable<Data.Cinema>> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(responseTask);
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = cinemasController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

       [TestMethod]
       public void CinemaService_GetCinemaById_ReturnCinema()
        {
            //Arrange
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockAuditoriumService = new Mock<IAuditoriumService>();

            Data.Cinema cinemas = _cinema;
            Task<Data.Cinema> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTask);
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = cinemasController.GetCinemaByIdAsync(responseTask.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resultAction.Id, responseTask.Result.Id);
        }

        [TestMethod]
        public void CinemaService_GetCinemaById_InsertedMockedNull_ReturnsNull()
        {
            //Arrange
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            IEnumerable<Data.Cinema> cinemas = null;
            Task<IEnumerable<Data.Cinema>> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<Data.Cinema>()));
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = cinemasController.GetCinemaByIdAsync(responseTask.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void CinemaService_DeleteCinema_InsertedMockedNull_ReturnNull()
        {
            //Arrange
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            IEnumerable<Data.Cinema> cinemas = null;
            Task<IEnumerable<Data.Cinema>> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository.Setup(x => x.Delete(It.IsAny<Data.Cinema>()));
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = cinemasController.DeleteCinema(responseTask.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void CinemaService_DeleteCinema_ReturnDeletedCinema()
        {
            //Arrange
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            List<Data.Cinema> mockedCinemas = new List<Data.Cinema>();
            mockedCinemas.Add(_cinema);
            IEnumerable<Data.Cinema> cinemas = mockedCinemas;
            Task<IEnumerable<Data.Cinema>> response = Task.FromResult(cinemas);
            Data.Cinema cinema = _cinema;
            Task<Data.Cinema> responseTask = Task.FromResult(cinema);
            DeleteAuditoriumDomainModel deleteAudit = _auditoriumDomainModel;
            Task<DeleteAuditoriumDomainModel> deleteResponse = Task.FromResult(deleteAudit);

            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(response);
            _mockAuditoriumService.Setup(x => x.DeleteAuditorium(_auditorium.Id)).Returns(deleteResponse);
            _mockCinemasRepository.Setup(x => x.Delete(responseTask.Result.Id)).Returns(responseTask.Result);
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = cinemasController.DeleteCinema(responseTask.Result.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resultAction.Cinema.Id, responseTask.Result.Id);

        }

        [TestMethod]
        public void CinemaService_UpdateCinema_Cinema()
        {
            //Arrange
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            List<Data.Cinema> cinemaModelsList = new List<Data.Cinema>();

            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()));
            _mockCinemasRepository.Setup(x => x.Update(It.IsAny<Data.Cinema>())).Returns(_cinema);
            _mockCinemasRepository.Setup(x => x.Save());
            CinemaService cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = cinemaService.UpdateCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resultAction.Id, _cinema.Id);
        }

        [TestMethod]
        public void CinemaService_UpdateCinema_InsertedMockedNull_ReturnNull()
        {
            //Arrange
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _cinema = null;

            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()));
            _mockCinemasRepository.Setup(x => x.Update(It.IsAny<Data.Cinema>())).Returns(_cinema);
            _mockCinemasRepository.Setup(x => x.Save());
            CinemaService cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = cinemaService.UpdateCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void CinemaService_CreateCinema_InsertMockedNull_ReturnErrorMessage()
        {
            //Arrange
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            List<Data.Cinema> cinemaModelsList = new List<Data.Cinema>();
            _cinema = null;
            string expectedMessage = "Error occured while creating a new cinema, please try again.";

            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<Data.Cinema>()));
            _mockCinemasRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Returns(_cinema);
            CinemaService cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);

            var resultAction = cinemaService.AddCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }


        [TestMethod]
        public void CinemaService_CreateCinema_InsertMocked_ReturnCinema()
        {
            //Arrange
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            List<Data.Cinema> cinemaModelsList = new List<Data.Cinema>();

            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()));
            _mockCinemasRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Returns(_cinema);
            _mockCinemasRepository.Setup(x => x.Save());
            CinemaService cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);

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
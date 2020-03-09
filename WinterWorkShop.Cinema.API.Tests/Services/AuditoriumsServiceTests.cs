using Microsoft.EntityFrameworkCore;
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
    public class AuditoriumsServiceTests
    {
        private Mock<IAuditoriumsRepository> _mockAuditoriumsRepository;
        private Mock<ICinemasRepository> _mockCinemasRepository;
        private Mock<ISeatsRepository> _seatsRepository;
        private Mock<IProjectionsRepository> _projectionsRepository;

        private Auditorium _auditorium;
        private WinterWorkShop.Cinema.Data.Cinema _cinema;

        private AuditoriumDomainModel _auditoriumDomainModel;
        private CinemaDomainModel _cinemaDomainModel;

        [TestInitialize]
        public void TestInitialize()
        {
            List<SeatDomainModel> seats = new List<SeatDomainModel>();
            foreach (var item in seats)
            {
                seats.Add(new SeatDomainModel
                {
                    Number = item.Number,
                    Row = item.Row,
                    Id = item.Id,
                    AuditoriumId = item.AuditoriumId
                });
            }

            //Doradi
            _auditorium = new Auditorium
            {
                Id = 1,
                CinemaId = 1,
                Name = "Naziv auditoriuma",
                Cinema = new Data.Cinema { Name = "imeBioskopa" },
               
            };

            _auditoriumDomainModel = new AuditoriumDomainModel
            {
               Id = 1,
               CinemaId = 1,
               Name = "Naziv auditoriuma",
               SeatsList = seats

            };

            _cinema = new WinterWorkShop.Cinema.Data.Cinema
            {
                 Id = 1,
                 Name = "Naziv bioskopa"
                
            };

            _cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "Naziv Bioskopa"

            };

            List<Auditorium> auditoriumsModelList = new List<Auditorium>();
            List<WinterWorkShop.Cinema.Data.Cinema> cinemaModelList = new List<WinterWorkShop.Cinema.Data.Cinema>();

            auditoriumsModelList.Add(_auditorium);
            cinemaModelList.Add(_cinema);

            IEnumerable<Auditorium> auditoriums = auditoriumsModelList;
            IEnumerable<WinterWorkShop.Cinema.Data.Cinema> cinemas = cinemaModelList;

            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoriums);
            Task<IEnumerable<WinterWorkShop.Cinema.Data.Cinema>> responseTask1 = Task.FromResult(cinemas);

            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockCinemasRepository = new Mock<ICinemasRepository>();


            _mockAuditoriumsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(responseTask1);

        }
/*
        [TestMethod]
        public void AuditoriumService_GetAllAsync_ReturnListOfAuditoriums()
        {
            //Arrange
            int expectedResultCount = 1;
            AuditoriumService auditoriumsService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object);
         // CinemaService cinemasService = new CinemaService(_mockCinemasRepository.Object);

            //Act
            var resultAction = auditoriumsService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
         // var resultAction1 = cinemasService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
           
            var result = (List<AuditoriumDomainModel>)resultAction;
         // var result1 = (List<CinemaDomainModel>)resultAction1;

            //Assert
            Assert.IsNotNull(result);
         // Assert.IsNotNull(result1);

            Assert.AreEqual(expectedResultCount, result.Count);
         // Assert.AreEqual(expectedResultCount, result1.Count);

            Assert.AreEqual(_auditorium.Id, result[0].Id);
         // Assert.AreEqual(_cinema.Id, result1[0].Id);

            Assert.IsInstanceOfType(result[0], typeof(AuditoriumDomainModel));
         // Assert.IsInstanceOfType(result1[0], typeof(CinemaDomainModel));

        }*/

        [TestMethod]
        public void AuditoriumService_GetAllAsync_ReturnNull()
        {
            IEnumerable<Auditorium> auditoriums = null;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoriums);

            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _seatsRepository.Object, _projectionsRepository.Object);

            var resultAction = auditoriumController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNull(resultAction);
        }


        [TestMethod]
        public void AuditoriumService_CreateAuditorium_InsertMockedNull_ReturnErrorMessage()
        {
            //Arrange
            List<Auditorium> auditoriumModelList = new List<Auditorium>();

            _auditorium = null;

            string expectedMessage = "Cannot create new auditorium, auditorium with given cinemaId does not exist.";

            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync (It.IsAny<int>()));
            _mockAuditoriumsRepository.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(_auditorium);

            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _seatsRepository.Object, _projectionsRepository.Object);

            //Act
            var resultAction = auditoriumController.CreateAuditorium(_auditoriumDomainModel, 1, 1).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }


       /* [TestMethod]
        public void AuditoriumService_CreateAuditorium_InsertMocked_ReturnAuditorium()
        {
            //Arrange
            List<Auditorium> auditoirumsModelsList = new List<Auditorium>();

            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetByAuditName(It.IsAny<string>(), It.IsAny<int>()));
            _mockAuditoriumsRepository.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(_auditorium);
            _mockAuditoriumsRepository.Setup(x => x.Save());
            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object);

            //Act
            var resultAction = auditoriumController.CreateAuditorium(_auditoriumDomainModel, 1, 1).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_auditorium.Id, resultAction.Auditorium.Id);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
        }*/

    }
}

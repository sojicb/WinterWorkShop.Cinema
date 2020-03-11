using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private Mock<IAuditoriumsRepository> _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
        private Mock<ICinemasRepository> _mockCinemasRepository = new Mock<ICinemasRepository>();
        private Mock<ISeatsRepository> _seatsRepository = new Mock<ISeatsRepository>();
        private Mock<IProjectionsRepository> _projectionsRepository = new Mock<IProjectionsRepository>();

        private Auditorium _auditorium;
        private Data.Cinema _cinema;

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
                Seats = new List<Seat>
                {
                    new Seat
                    {
                        Id = new Guid(),
                        Number = 1,
                        Row = 1,
                        AuditoriumId = 1
                    }
                }
            };

            _auditoriumDomainModel = new AuditoriumDomainModel
            {
               Id = 1,
               CinemaId = 1,
               Name = "Naziv auditoriuma",
               SeatsList = seats

            };

            _cinema = new Data.Cinema
            {
                 Id = 1,
                 Name = "Naziv bioskopa"
                
            };

            _cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "Naziv Bioskopa"

            };

        }

        [TestMethod]
        public void AuditoriumService_GetAllAsync_ReturnListOfAuditoriums()
        {
            //Arrange
            int expectedResult = 1;
            List<Auditorium> auditoria = new List<Auditorium>();
            auditoria.Add(_auditorium);
            IEnumerable<Auditorium> auditoriums = auditoria;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoriums);

            _mockAuditoriumsRepository.Setup(x => x.GetAll()).Returns(responseTask);

            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _seatsRepository.Object, _projectionsRepository.Object);

            //Act
            var resultAction = auditoriumController.GetAllAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()
                .ToList();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resultAction.Count, expectedResult);
        }

        [TestMethod]
        public void AuditoriumService_GetAllAsync_InsertedMockedNull_ReturnsNull()
        {
            //Arrange
            IEnumerable<Auditorium> auditoriums = null;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoriums);

            _mockAuditoriumsRepository.Setup(x => x.GetAll()).Returns(responseTask);

            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _seatsRepository.Object, _projectionsRepository.Object);

            //Act
            var resultAction = auditoriumController.GetAllAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void AuditoriumService_CreateAuditorium_ReturnCreatedAuditorium()
        {
            //Arrange
            int numOfSeats = 1;
            int numOfRows = 1;
            Auditorium auditorium = _auditorium;
            Task<Auditorium> responseTask = Task.FromResult(auditorium);
            Data.Cinema cinema = new Data.Cinema();
            Task<Data.Cinema> response = Task.FromResult(cinema);


            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()));
            _mockAuditoriumsRepository.Setup(x => x.GetByAuditName(It.IsAny<string>(), It.IsAny<int>()));
            _mockAuditoriumsRepository.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(responseTask.Result);

            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _seatsRepository.Object, _projectionsRepository.Object);

            //Act
            var resultAction = auditoriumController.CreateAuditorium(_auditoriumDomainModel, numOfSeats, numOfRows)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resultAction.Auditorium.Id, responseTask.Result.Id);

        }

        [TestMethod]
        public void AuditoriumService_CreateAuditorium_InsertMockedNull_ReturnErrorCinemaIdDoesNotExist()
        {
            //Arrange
            List<Auditorium> auditoriumModelList = new List<Auditorium>();

            Auditorium auditorium = _auditorium;
            Task<Auditorium> responseTask = Task.FromResult(auditorium);
            Data.Cinema cinema = new Data.Cinema();
            Task<Data.Cinema> response = Task.FromResult(cinema);

            _auditorium = null;

            string expectedMessage = "Cannot create new auditorium, auditorium with given cinemaId does not exist.";

            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync(response.Result.Id));
            _mockAuditoriumsRepository.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(_auditorium);

            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _seatsRepository.Object, _projectionsRepository.Object);

            //Act
            var resultAction = auditoriumController.CreateAuditorium(_auditoriumDomainModel, 1, 1).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }
        [TestMethod]
        public void AuditoriumService_CreateAuditorium_InsertMockedNull_ReturnErrorAuditoriumWithSameName()
        {
            //Arrange
            List<Auditorium> auditoriumModelList = new List<Auditorium>();

            _auditorium = null;

            string expectedMessage = "Cannot create new auditorium, auditorium with same name alredy exist.";

            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetByAuditName(It.IsAny<string>(), It.IsAny<int>()));
            _mockAuditoriumsRepository.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(_auditorium);

            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _seatsRepository.Object, _projectionsRepository.Object);

            //Act
            var resultAction = auditoriumController.CreateAuditorium(_auditoriumDomainModel, 1, 1).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }

        [TestMethod]
        public void AuditoriumService_CreateAuditorium_InsertMockedNull_ReturnErrorAuditoriumCreationError()
        {
            //Arrange
            List<Auditorium> auditoriumModelList = new List<Auditorium>();

            _auditorium = null;

            Auditorium auditorium = _auditorium;
            Task<Auditorium> responseTask = Task.FromResult(auditorium);
            Data.Cinema cinema = new Data.Cinema();
            Task<Data.Cinema> response = Task.FromResult(cinema);

            string expectedMessage = "Error occured while creating new auditorium, please try again.";

            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync(response.Result.Id));
            _mockAuditoriumsRepository.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(_auditorium);

            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _seatsRepository.Object, _projectionsRepository.Object);

            //Act
            var resultAction = auditoriumController.CreateAuditorium(_auditoriumDomainModel, 1, 1).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }



        [TestMethod]
        public void AuditoriumService_CreateAuditorium_InsertedMockedNull_ReturnNull()
        {
            //Arrange
            
            //Act
            

            //Assert

        }

        [TestMethod]
        public void AuditoriumService_GetAuditoriumById_ReturnAuditoriumById()
        {
            //Arrange
            Auditorium auditorium = _auditorium;
            Task<Auditorium> responseTask = Task.FromResult(auditorium);

            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTask);

            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _seatsRepository.Object, _projectionsRepository.Object);

            //Act
            var resultAction = auditoriumController.GetAuditoriumByIdAsync(_auditorium.Id)
               .ConfigureAwait(false)
               .GetAwaiter()
               .GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resultAction.Id, responseTask.Result.Id);
        }

        [TestMethod]
        public void AuditoriumService_GetAuditoriumById_InsertedMockedNull_ReturnNull()
        {
            //Arrange
            Auditorium auditorium = null;
            Task<Auditorium> responseTask = Task.FromResult(auditorium);

            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTask);

            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _seatsRepository.Object, _projectionsRepository.Object);

            //Act
            var resultAction = auditoriumController.GetAuditoriumByIdAsync(_auditorium.Id)
               .ConfigureAwait(false)
               .GetAwaiter()
               .GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void AuditoriumService_UpdateAuditorium_ReturnUpdatedAuditorium()
        {
            Auditorium auditorium = _auditorium;
            Task<Auditorium> responseTask = Task.FromResult(auditorium);

            _mockAuditoriumsRepository.Setup(x => x.Update(It.IsAny<Auditorium>())).Returns(responseTask.Result);

            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _seatsRepository.Object, _projectionsRepository.Object);

            //Act
            var resultAction = auditoriumController.UpdateAuditorium(_auditoriumDomainModel)
               .ConfigureAwait(false)
               .GetAwaiter()
               .GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resultAction.Id, responseTask.Result.Id);
        }

        [TestMethod]
        public void AuditoriumService_UpdateAuditorium_InsertMockedNull_ReturnNull()
        {
            Auditorium auditorium = null;
            Task<Auditorium> responseTask = Task.FromResult(auditorium);

            _mockAuditoriumsRepository.Setup(x => x.Update(It.IsAny<Auditorium>())).Returns(responseTask.Result);

            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _seatsRepository.Object, _projectionsRepository.Object);

            //Act
            var resultAction = auditoriumController.UpdateAuditorium(_auditoriumDomainModel)
               .ConfigureAwait(false)
               .GetAwaiter()
               .GetResult();

            //Assert
            Assert.IsNull(resultAction);

        }

        //[TestMethod]
        //public void AuditoriumService_DeleteAuditorium_ReturnDeletedAuditorium()
        //{
        //    //Arrange

        //    //Act


        //    //Assert

        //}

        //[TestMethod]
        //public void AuditoriumService_DeleteAuditorium_InsertMockedNull_ReturnNull()
        //{
        //    //Arrange

        //    //Act


        //    //Assert

        //}


        [TestMethod]
        public void AuditoriumService_GetAllByCinemaId_ReturnAuditoriums()
        {
            //Arrange
            int expectedResult = 1;
            List<Auditorium> MockedAuditoria = new List<Auditorium>();
            MockedAuditoria.Add(_auditorium);
            IEnumerable<Auditorium> auditoria = MockedAuditoria;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoria);

            _mockAuditoriumsRepository.Setup(x => x.GetAll()).Returns(responseTask);

            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _seatsRepository.Object, _projectionsRepository.Object);

            //Act
            var resultAction = auditoriumController.GetAllByCinemaId(_cinema.Id)
               .ConfigureAwait(false)
               .GetAwaiter()
               .GetResult()
               .ToList();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resultAction.Count, expectedResult);
        }

        [TestMethod]
        public void AuditoriumService_GetAllByCinemaId_InsertedMockedNull_ReturnNull()
        {
            //Arrange
            IEnumerable<Auditorium> auditoria = null;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoria);

            _mockAuditoriumsRepository.Setup(x => x.GetAll()).Returns(responseTask);

            AuditoriumService auditoriumController = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _seatsRepository.Object, _projectionsRepository.Object);

            //Act
            var resultAction = auditoriumController.GetAllByCinemaId(_cinema.Id)
               .ConfigureAwait(false)
               .GetAwaiter()
               .GetResult();

            //Assert
            Assert.IsNull(resultAction);
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

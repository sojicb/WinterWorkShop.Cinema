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
            var result = (List<CinemaDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_cinema.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(CinemaDomainModel));
        }

        //[TestMethod]
        //public void CinemaService_GetCinemasByName_ReturnsACinema()
        //{
        //    //Arrange
        //    int expectedResultCount = 1;
        //    _cinema.Name = "Zrenjanin";
        //    CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object);

        //    //Act
        //    var resultAction = cinemasController
        //        .GetCinemaByName("Zrenjanin")
        //        .ConfigureAwait(false)
        //        .GetAwaiter()
        //        .GetResult();
        //    var result = (CinemaDomainModel)resultAction;

        //    //Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(expectedResultCount, result.Id);
        //    Assert.AreEqual(_cinema.Name, result.Name);
        //    Assert.IsInstanceOfType(result, typeof(CinemaDomainModel));
        //}

        [TestMethod]
        public void CinemaService_GetCinemasByName_ReturnNull()
        {
            //Arrange
            Data.Cinema cinemas = null;
            Task<Data.Cinema> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByCinemaName("")).Returns(responseTask);
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object);

            //Act
            var resultAction = cinemasController
                .GetCinemaByName("")
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            //Assert
            Assert.IsNull(resultAction);
            Assert.AreEqual(cinemas, resultAction);
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
        [ExpectedException(typeof(DbUpdateException))]
        public void CinemaService_CreateCinema_When_Updating_Non_Existing_Cinema()
        {
            // Arrange
            List<Data.Cinema> cinemasModelsList = new List<Data.Cinema>();

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Throws(new DbUpdateException());
            _mockCinemasRepository.Setup(x => x.Save());
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object);

            //Act
            var resultAction = cinemasController.AddCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();


            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsFalse(resultAction.IsSuccessful);
        }

        [TestMethod]
        public void CinemaService_GetCinemasById_ReturnCinema()
        {
            //Arrange
            Data.Cinema cinemas = new Data.Cinema()
            {
                Id = 0,
                Name = "Randomz"
            };
            Task<Data.Cinema> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(cinemas.Id)).Returns(responseTask);
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object);

            //Act
            var resultAction = cinemasController
                .GetCinemaByIdAsync(0)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(cinemas.Id, resultAction.Id);
            Assert.IsInstanceOfType(resultAction, typeof(CinemaDomainModel));
        }

        [TestMethod]
        public void CinemaService_GetCinemasById_ReturnNull()
        {
            //Arrange
            Data.Cinema cinemas = null;
            Task<Data.Cinema> responseTask = Task.FromResult(cinemas);

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(null)).Returns(responseTask);
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object);

            //Act
            var resultAction = cinemasController
                .GetCinemaByIdAsync(0)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            //Assert
            Assert.IsNull(resultAction);
            Assert.AreEqual(cinemas, resultAction);
        }

        //[TestMethod]
        //public void CinemaService_UpdatingCinema_ReturnCinema()
        //{
        //    //Arrange
        //    Data.Cinema cinemas = new Data.Cinema()
        //    {
        //        Id = 0,
        //        Name = "Randomz"
        //    };

        //    Task<Data.Cinema> responseTask = Task.FromResult(cinemas);

        //    _mockCinemasRepository = new Mock<ICinemasRepository>();
        //    _mockCinemasRepository.Setup(x => x.Update(cinemas)).Returns(cinemas);
        //    CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object);

        //    CinemaDomainModel domainModel = new CinemaDomainModel()
        //    {
        //        Id = 0,
        //        Name = "Rendomz123"
        //    };

        //    //Act
        //    var resultAction = cinemasController
        //        .UpdateCinema(domainModel)
        //        .ConfigureAwait(false)
        //        .GetAwaiter()
        //        .GetResult();

        //    //Assert
        //    Assert.IsNotNull(resultAction);
        //    Assert.AreNotEqual(cinemas.Name, resultAction.Name);
        //    Assert.IsInstanceOfType(resultAction, typeof(CinemaDomainModel));
        //}

        [TestMethod]
        public void CinemaService_DeletingCinema_ReturnsACinema()
        {
            //Arrange
            Data.Cinema cinemas = new Data.Cinema()
            {
                Id = 0,
                Name = "Randomz"
            };

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockCinemasRepository.Setup(x => x.Delete(cinemas.Id)).Returns(cinemas);
            CinemaService cinemasController = new CinemaService(_mockCinemasRepository.Object);

            //Act
            var resultAction = cinemasController
                .DeleteCinema(0)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(cinemas.Id, resultAction.Id);
            Assert.IsInstanceOfType(resultAction, typeof(CinemaDomainModel));
            Assert.AreEqual(cinemas.Name, resultAction.Name);
        }
    }
}

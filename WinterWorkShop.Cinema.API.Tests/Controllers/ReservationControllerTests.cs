using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class ReservationControllerTests
    {
        private Mock<IReservationService> _reservationService;

        [TestMethod]
        public void GetAsync_Return_All_Reservations()
        {
            //Arrange
            List<ReservationDomainModel> reservationsDomainModelsList = new List<ReservationDomainModel>();
            ReservationDomainModel reservationDomainModel = new ReservationDomainModel
            {
               Id = Guid.NewGuid(),
               Username = "username",
               UserId = Guid.NewGuid(),
               MovieTitle = "Movie title",
               ProjectionId = Guid.NewGuid(),
               ProjectionTime = DateTime.Now.AddDays(1),
               AuditoriumId = 1
            };

            reservationsDomainModelsList.Add(reservationDomainModel);
            IEnumerable<ReservationDomainModel> reservationDomainModels = reservationsDomainModelsList;
            Task<IEnumerable<ReservationDomainModel>> responseTask = Task.FromResult(reservationDomainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _reservationService = new Mock<IReservationService>();
            _reservationService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            ReservationsController reservationController = new ReservationsController(_reservationService.Object);

            //Act
            var result = reservationController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var reservationDomainModelResultList = (List<ReservationDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(reservationDomainModelResultList);
            Assert.AreEqual(expectedResultCount, reservationDomainModelResultList.Count);
            Assert.AreEqual(reservationDomainModel.Id, reservationDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void PostAsync_Create_Throw_DbException_Reservation()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;

            CreateReservationModel createResevation = new CreateReservationModel()
            {
                UserId = Guid.NewGuid(),
                ProjectionId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };
            CreateReservationResultModel createReservationResultModel = new CreateReservationResultModel
            {
                Reservation = new ReservationDomainModel
                {
                    Id = Guid.NewGuid(),
                    Username = "username",
                    UserId = Guid.NewGuid(),
                    MovieTitle = "Movie title",
                    ProjectionId = Guid.NewGuid(),
                    ProjectionTime = DateTime.Now.AddDays(1),
                    AuditoriumId = 1
                },
                IsSuccessful = true
            };
            Task<CreateReservationResultModel> responseTask = Task.FromResult(createReservationResultModel);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _reservationService = new Mock<IReservationService>();
            _reservationService.Setup(x => x.HandleSeatValidation(It.IsAny<ReservationDomainModel>()));
            _reservationService.Setup(x => x.CreateReservation(It.IsAny<ReservationDomainModel>())).Throws(dbUpdateException);
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object);

            //Act
            var result = reservationsController.CreateReservation(createResevation).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }


        [TestMethod]
        public void PostAsync_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;

            CreateReservationModel createReservationModel = new CreateReservationModel()
            {
                UserId = Guid.NewGuid(),
                ProjectionId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };

            _reservationService = new Mock<IReservationService>();
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object);
            reservationsController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = reservationsController.CreateReservation(createReservationModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void PostAsync_With_UnValid_ProjectionDate_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Projection time cannot be in past.";
            int expectedStatusCode = 400;

            CreateReservationModel createReservationModel = new CreateReservationModel()
            {
                UserId = Guid.NewGuid(),
                ProjectionId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(-1),
                AuditoriumId = 1
            };

            _reservationService = new Mock<IReservationService>();
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object);

            //Act
            var result = reservationsController.CreateReservation(createReservationModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault(nameof(createReservationModel.ProjectionTime));
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void PostAsync_Create_createProjectionResultModel_IsSuccessful_True_Projection()
        {
            //Arrange
            int expectedStatusCode = 201;

            CreateReservationModel createReservationModel = new CreateReservationModel()
            {
                UserId = Guid.NewGuid(),
                ProjectionId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };
            CreateReservationResultModel createReservationResultModel = new CreateReservationResultModel
            {
                Reservation = new ReservationDomainModel
                {
                    Id = Guid.NewGuid(),
                    Username = "username",
                    UserId = Guid.NewGuid(),
                    MovieTitle = "Movie title",
                    ProjectionId = Guid.NewGuid(),
                    ProjectionTime = DateTime.Now.AddDays(1),
                    AuditoriumId = 1
                },
                IsSuccessful = true
            };
            Task<CreateReservationResultModel> responseTask = Task.FromResult(createReservationResultModel);

            _reservationService = new Mock<IReservationService>();
            _reservationService.Setup(x => x.HandleSeatValidation(It.IsAny<ReservationDomainModel>()));
            _reservationService.Setup(x => x.CreateReservation(It.IsAny<ReservationDomainModel>())).Returns(responseTask);
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object);

            //Act
            var result = reservationsController.CreateReservation(createReservationModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var createdResult = ((CreatedResult)result).Value;
            var reservationDomainModel = (ReservationDomainModel)createdResult;

            //Assert
            Assert.IsNotNull(reservationDomainModel);
            Assert.AreEqual(createReservationModel.AuditoriumId, reservationDomainModel.AuditoriumId);
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)result).StatusCode);
        }
    }
}

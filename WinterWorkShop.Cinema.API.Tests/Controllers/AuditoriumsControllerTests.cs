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
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class AuditoriumsControllerTests
    {
        private Mock<IAuditoriumService> _auditoriumService;


        [TestMethod]
        public void GetAsyn_Return_All_Auditoriums()
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


            //Arange
            List<AuditoriumDomainModel> auditoriumDomainModelsList = new List<AuditoriumDomainModel>();
            AuditoriumDomainModel auditoriumDomainModel = new AuditoriumDomainModel
            {
                Id = 1,
                Name = "AuditoriumName",
                CinemaId = 1,
                SeatsList = seats
               

            };

            auditoriumDomainModelsList.Add(auditoriumDomainModel);
            IEnumerable<AuditoriumDomainModel> auditoriumDomainModels = auditoriumDomainModelsList;
            Task<IEnumerable<AuditoriumDomainModel>> responseTask = Task.FromResult(auditoriumDomainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _auditoriumService = new Mock<IAuditoriumService>();
            _auditoriumService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_auditoriumService.Object);


            //Act
            var result = auditoriumsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var auditoriumsDomainModelResultList = (List<AuditoriumDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(auditoriumDomainModelsList);
            Assert.AreEqual(expectedResultCount, auditoriumDomainModelsList.Count);
            Assert.AreEqual(auditoriumDomainModel.Id, auditoriumDomainModelsList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }


        [TestMethod]
        public void GetAsyncById_Return_Auditorium()
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

            //Arange
            AuditoriumDomainModel auditoriumDomainModel = new AuditoriumDomainModel
            {
                Id = 1,
                CinemaId = 1,
                Name = "Audit name",
                SeatsList = seats
            };

            Task<AuditoriumDomainModel> responseTask = Task.FromResult(auditoriumDomainModel);
            int expectedStatusCode = 200;

            _auditoriumService = new Mock<IAuditoriumService>();
            _auditoriumService.Setup(x => x.GetAuditoriumByIdAsync(It.IsAny<int>())).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_auditoriumService.Object);

            //Act
            var result = auditoriumsController.GetAsyncById(auditoriumDomainModel.Id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var auditoriumsDomainModelResultList = (AuditoriumDomainModel)resultList;

            //Assert
            Assert.IsNotNull(auditoriumDomainModel);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);

        }

        [TestMethod]
        public void GetAsyncById_Return_AuditoriumDoesNotExist()
        {
            //Arange
            int id = 0;

            AuditoriumDomainModel auditoriumDomainModel = null;
            Task<AuditoriumDomainModel> responseTask = Task.FromResult(auditoriumDomainModel);
            int expectedStatusCode = 404;

            _auditoriumService = new Mock<IAuditoriumService>();
            _auditoriumService.Setup(x => x.GetAuditoriumByIdAsync(id)).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_auditoriumService.Object);

            //Act
            var result = auditoriumsController.GetAsyncById(id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)result).StatusCode);

        }

        [TestMethod]
        public void Put_UpdateAuditorium_Return_UpdatedAuditorium()
        {
            //Arange
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

            AuditoriumDomainModel auditoriumDomainModel = new AuditoriumDomainModel
            {
                Id = 1,
                CinemaId = 1,
                Name = "Audit name",
                SeatsList = seats
            };

            UpdateAuditoriumModel updateAuditoriumModel = new UpdateAuditoriumModel 
            {
               cinemaId = 2,
               Name = "Updated name"
            };

            Task<AuditoriumDomainModel> responseTask = Task.FromResult(auditoriumDomainModel);
            int expectedStatusCode = 202;

            _auditoriumService = new Mock<IAuditoriumService>();
            _auditoriumService.Setup(x => x.GetAuditoriumByIdAsync(It.IsAny<int>())).Returns(responseTask);
            _auditoriumService.Setup(x => x.UpdateAuditorium(auditoriumDomainModel)).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_auditoriumService.Object);

            //Act
            var result = auditoriumsController.Put(auditoriumDomainModel.Id, updateAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultList = ((AcceptedResult)result).Value;
            var auditoriumsDomainModelResultList = (AuditoriumDomainModel)resultList;

            //Assert
            Assert.IsNotNull(auditoriumsDomainModelResultList);
            Assert.AreEqual(auditoriumDomainModel.Id, auditoriumsDomainModelResultList.Id);
            Assert.IsInstanceOfType(result, typeof(AcceptedResult));
            Assert.AreEqual(expectedStatusCode, ((AcceptedResult)result).StatusCode);
        }   


        [TestMethod]
        public void GetAllByCinemaId_Return_AuditoirumsByCinemaId()
        {
            AuditoriumDomainModel auditoriumDomainModel = new AuditoriumDomainModel
            {
                Id = 1,
                CinemaId = 1,
                Name = "Audit name",
            };

            Task<AuditoriumDomainModel> responseTask = Task.FromResult(auditoriumDomainModel);
            int expectedStatusCode = 200;

            _auditoriumService = new Mock<IAuditoriumService>();
            _auditoriumService.Setup(x => x.GetAllByCinemaId(auditoriumDomainModel.CinemaId));
            AuditoriumsController auditoriumsController = new AuditoriumsController(_auditoriumService.Object);

            //Act
            var result = auditoriumsController.GetAllByCinemaId(auditoriumDomainModel.CinemaId).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
           

            //Assert
            Assert.IsNotNull(auditoriumDomainModel);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_NewList()
        {
            //Arrange
            IEnumerable<AuditoriumDomainModel> auditoriumDomainModels = null;
            Task<IEnumerable<AuditoriumDomainModel>> responseTask = Task.FromResult(auditoriumDomainModels);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _auditoriumService = new Mock<IAuditoriumService>();
            _auditoriumService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_auditoriumService.Object);


            //Act
            var result = auditoriumsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var auditoriumsDomainModelResultList = (List<AuditoriumDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(auditoriumsDomainModelResultList);
            Assert.AreEqual(expectedResultCount, auditoriumsDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }


        [TestMethod]
        public void PostAsync_Create_createAuditoriumResultModel_IsSuccessful_True_Auditoriums()
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

            //Arrange
            int expectedStatusCode = 201;

            CreateAuditoriumModel createAuditoriumModel = new CreateAuditoriumModel()
            {
                cinemaId = 1,
                auditName = "Ime auditoriuma",
                seatRows = 2,
                numberOfSeats = 2
            };

            CreateAuditoriumResultModel createAuditoriumResultModel = new CreateAuditoriumResultModel()
            {
                Auditorium = new AuditoriumDomainModel
                {
                    Id = 1,
                    CinemaId = 1,
                    Name = "Ime Auditoriuma",
                    SeatsList = seats
                },

                IsSuccessful = true
            };

            Task<CreateAuditoriumResultModel> responseTask = Task.FromResult(createAuditoriumResultModel);

            _auditoriumService = new Mock<IAuditoriumService>();
            _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_auditoriumService.Object);

            //Act
            var result = auditoriumsController.PostAsync(createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var createResult = ((CreatedResult)result).Value;
            var auditoriumDomainModel = (CreateAuditoriumResultModel)createResult;

            //Assert
            Assert.IsNotNull(auditoriumDomainModel);
            Assert.AreEqual(createAuditoriumModel.cinemaId, auditoriumDomainModel.Auditorium.CinemaId);
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)result).StatusCode);
        }


        [TestMethod]
        public void PostAsync_Create_Trow_DbException_Auditorium()
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

            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;


            CreateAuditoriumModel createAuditoriumModel = new CreateAuditoriumModel()
            {
                cinemaId = 1,
                auditName = "Ime auditoriuma",
                numberOfSeats = 2,
                seatRows = 2
            };

            CreateAuditoriumResultModel createAuditoriumResultModel = new CreateAuditoriumResultModel()
            {
                Auditorium = new AuditoriumDomainModel
                {
                    Id = 1,
                    Name = "Ime auditoriuma",
                    CinemaId = 1,
                    SeatsList = seats
                },
                IsSuccessful = true
            };


            Task<CreateAuditoriumResultModel> responseTask = Task.FromResult(createAuditoriumResultModel);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);


            _auditoriumService = new Mock<IAuditoriumService>();
            _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(),It.IsAny<int>(),It.IsAny<int>())).Throws(dbUpdateException);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_auditoriumService.Object);

            //Act
            var result = auditoriumsController.PostAsync(createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
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
        public void PostAsync_Create_createAuditoriumResultModel_IsSuccessful_False_Return_BadRequest()
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
                
            //Arrange
            string expectedMessage = "Error occured while creating new auditorium, please try again.";
            int expectedStatusCode = 400;

            CreateAuditoriumModel createAuditoriumModel = new CreateAuditoriumModel()
            {
                cinemaId = 1,
                auditName = "Ime auditoriuma",
                numberOfSeats = 2,
                seatRows = 2
            };

            CreateAuditoriumResultModel createAuditoriumResultModel = new CreateAuditoriumResultModel()
            {
                Auditorium = new AuditoriumDomainModel
                {
                    Id = 5,
                    Name = "neko ime auditoriuma",
                    CinemaId = createAuditoriumModel.cinemaId,
                    SeatsList = seats
                },
                IsSuccessful = false,
                ErrorMessage = Messages.AUDITORIUM_CREATION_ERROR,
            };

            Task<CreateAuditoriumResultModel> responseTask = Task.FromResult(createAuditoriumResultModel);

            _auditoriumService = new Mock<IAuditoriumService>();
            _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(responseTask);
            AuditoriumsController auditoriumsController = new AuditoriumsController(_auditoriumService.Object);

            //Act
            var result = auditoriumsController.PostAsync(createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
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

            CreateAuditoriumModel createAuditoriumModel = new CreateAuditoriumModel()
            {
               cinemaId = 1,
               auditName = "Ime auditoriuma",
               numberOfSeats = 3,
               seatRows = 3
            };

            _auditoriumService = new Mock<IAuditoriumService>();
            AuditoriumsController auditoriumsController = new AuditoriumsController(_auditoriumService.Object);
            auditoriumsController.ModelState.AddModelError("key","Invalid Model State");

            //Act
            var result = auditoriumsController.PostAsync(createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
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

    }
}

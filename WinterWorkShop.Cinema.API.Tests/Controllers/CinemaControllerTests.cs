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

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class CinemaControllerTests
    {
        private Mock<ICinemaService> _cinemaService = new Mock<ICinemaService>();
        private CinemaModels _model;
        private CinemaDomainModel _domainModel;
        private DeleteCinemaDomainModel _deleteCinema;
        private DeleteCinemaDomainModel _deletedCinema;
        private DeleteCinemaDomainModel _deletedCinemaError;

        [TestInitialize]
        public void TestInitialize()
        {
            _domainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "CinemaProba",
            };

            _model = new CinemaModels
            {
                Name = "Proba"
            };

            _deleteCinema = new DeleteCinemaDomainModel
            {
                ErrorMessage = "Cinema cannot be deleted due to projections in the future",
                IsSuccessful = false
            };

            _deletedCinema = new DeleteCinemaDomainModel
            {
                Cinema = _domainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };

            _deletedCinemaError = new DeleteCinemaDomainModel
            {
                Cinema = null,
                ErrorMessage = null,
                IsSuccessful = true
            };
        }

        [TestMethod]
        public void GetAsync_Return_All_Cinemas()
        {
            //Arrange
            List<CinemaDomainModel> cinemaDomainModelsList = new List<CinemaDomainModel>();
            CinemaDomainModel cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "Cinema name"
            };

            cinemaDomainModelsList.Add(cinemaDomainModel);
            IEnumerable<CinemaDomainModel> cinemaDomainModels = cinemaDomainModelsList;
            Task<IEnumerable<CinemaDomainModel>> responseTask = Task.FromResult(cinemaDomainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _cinemaService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);

            //Act
            var result = cinemasController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var cinemaDomainModelResultList = (List<CinemaDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(cinemaDomainModelResultList);
            Assert.AreEqual(expectedResultCount, cinemaDomainModelResultList.Count);
            Assert.AreEqual(cinemaDomainModel.Id, cinemaDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }


        [TestMethod]
        public void GetAsyncById_Return_CinemaById()
        {
            //Arrange
            CinemaDomainModel cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "Cinema name"
            };


            Task<CinemaDomainModel> responseTask = Task.FromResult(cinemaDomainModel);
            int expectedStatusCode = 200;

            _cinemaService.Setup(x => x.GetCinemaByIdAsync(It.IsAny<int>())).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);

            //Act
            var result = cinemasController.GetAsyncById(cinemaDomainModel.Id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var cinemaDomainModelResultList = (CinemaDomainModel)resultList;

            //Assert
            Assert.IsNotNull(cinemaDomainModelResultList);
            Assert.AreEqual(cinemaDomainModel.Id, cinemaDomainModelResultList.Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void CinemaCotroller_GetAsyncById_InsertedNUll_ReturnsNull()
        {
            //Arrange
            int id = 1;
            int expectedStatusCode = 404;
            CinemaDomainModel cinemaModel = null;
            Task<CinemaDomainModel> responseTask = Task.FromResult(cinemaModel);

            _cinemaService.Setup(x => x.GetCinemaByIdAsync(It.IsAny<int>())).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);

            //Act
            var result = cinemasController.GetAsyncById(id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultModel = ((NotFoundObjectResult)result);

            //Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, resultModel.StatusCode);
        }


        [TestMethod]
        public void GetAsync_Return_NewList()
        {
            //Arrange
            IEnumerable<CinemaDomainModel> cinemaDomainModels = null;
            Task<IEnumerable<CinemaDomainModel>> responseTask = Task.FromResult(cinemaDomainModels);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);

            //Act
            var result = cinemasController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var cinemaDomainModelResultList = (List<CinemaDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(cinemaDomainModelResultList);
            Assert.AreEqual(expectedResultCount, cinemaDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetByIdAsync_Return_CinemaNotFound()
        {

            //Arrange
            int Id = 1;

            CinemaDomainModel cinemaDomainModel = null;
            Task<CinemaDomainModel> responseTask = Task.FromResult(cinemaDomainModel);
            int expectedStatusCode = 404;

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetCinemaByIdAsync(Id)).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);

            //Act
            var result = cinemasController.GetAsyncById(Id).ConfigureAwait(false).GetAwaiter().GetResult().Result;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_Null()
        {
            //Arrange
            IEnumerable<CinemaDomainModel> cinemaDomainModels = null;
            Task<IEnumerable<CinemaDomainModel>> responseTask = Task.FromResult(cinemaDomainModels);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);

            //Act
            var result = cinemasController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var cinemaDomainModelResultList = (List<CinemaDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(cinemaDomainModelResultList);
            Assert.AreEqual(expectedResultCount, cinemaDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void PostAsync_Create_Throw_DbExcpetion()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;

            CinemaModels cinemaModels = new CinemaModels
            {
                Name = "Cinema name"
            };

            CreateCinemaResultModel createCinemaResultModel = new CreateCinemaResultModel
            {
                Cinema = new CinemaDomainModel
                {
                    Id = 1,
                    Name = "Cinema name"
                },
               
                IsSuccessful = true

            };

            Task<CreateCinemaResultModel> responseTask = Task.FromResult(createCinemaResultModel);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.AddCinema(It.IsAny<CinemaDomainModel>())).Throws(dbUpdateException);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);

            //Act
            var result = cinemasController.Post(cinemaModels).ConfigureAwait(false).GetAwaiter().GetResult();
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

            CinemaModels cinemaModels = new CinemaModels()
            {
                Name = "Cinema name"
            };

            _cinemaService = new Mock<ICinemaService>();
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);
            cinemasController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = cinemasController.Post(cinemaModels).ConfigureAwait(false).GetAwaiter().GetResult();
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

        //Doradi
        [TestMethod]
        public void PutAsync_GetCinemaByIdAsync_Return_UpdatedCinema()
        {
            //Arrange
            CinemaDomainModel cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "Cinema name"
            };

            CinemaModels cinemaModels = new CinemaModels
            {
                Name = cinemaDomainModel.Name
            };

            Task<CinemaDomainModel> responseTask = Task.FromResult(cinemaDomainModel);
            int expectedStatusCode = 202;

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetCinemaByIdAsync(It.IsAny<int>())).Returns(responseTask);
            _cinemaService.Setup(x => x.UpdateCinema(cinemaDomainModel)).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);

            //Act
            var result = cinemasController.Put(cinemaDomainModel.Id, cinemaModels).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultList = ((AcceptedResult)result).Value;
            var cinemaDomainModelResultList = (CinemaDomainModel)resultList;

            //Assert
            Assert.IsNotNull(cinemaDomainModelResultList);
            Assert.AreEqual(cinemaDomainModel.Id, cinemaDomainModelResultList.Id);
            Assert.IsInstanceOfType(result, typeof(AcceptedResult));
            Assert.AreEqual(expectedStatusCode, ((AcceptedResult)result).StatusCode);
        }


        [TestMethod]
        public void PostAsync_Create_createCinemaResultModel_IsSuccessful_True_Cinema()
        {

            //Arrange
            int expectedStatusCode = 201;

            CinemaModels cinemaModel = new CinemaModels()
            {
                Name = "Cinema name"
            };

            CreateCinemaResultModel createCinemaResultModel = new CreateCinemaResultModel()
            {
                Cinema = new CinemaDomainModel
                {
                   Id = 1,
                   Name = "Cinema name"
                },

                IsSuccessful = true
            };

            Task<CreateCinemaResultModel> responseTask = Task.FromResult(createCinemaResultModel);

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.AddCinema(It.IsAny<CinemaDomainModel>())).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);

            //Act
            var result = cinemasController.Post(cinemaModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var createResult = ((CreatedResult)result).Value;
            var auditoriumDomainModel = (CinemaDomainModel)createResult;

            //Assert
            Assert.IsNotNull(cinemaModel);
            Assert.AreEqual(cinemaModel.Name, auditoriumDomainModel.Name);
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)result).StatusCode);
        }

        [TestMethod]
        public void CinemaCotroller_Delete_ReturnsDeletedCinema()
        {
            //Arrange
            DeleteCinemaDomainModel cinemaModel = _deletedCinema;
            Task<DeleteCinemaDomainModel> responseTask = Task.FromResult(cinemaModel);

            _cinemaService.Setup(x => x.DeleteCinema(responseTask.Result.Cinema.Id)).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);

            //Act
            var result = cinemasController.Delete(responseTask.Result.Cinema.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(AcceptedResult));

        }

        [TestMethod]
        public void CinemaCotroller_Delete_InsertNull_ReturnsBadRequest()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int id = 1;
            int expectedStatusCode = 400;
            DeleteCinemaDomainModel cinemaModel = null;
            Task<DeleteCinemaDomainModel> responseTask = Task.FromResult(cinemaModel);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _cinemaService.Setup(x => x.DeleteCinema(It.IsAny<int>())).Throws(dbUpdateException);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);

            //Act
            var result = cinemasController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void CinemaCotroller_Delete_InsertNull_ReturnsException_CinemaNotFound()
        {
            //Arrange
            string expectedMessage = "Cinema does not exist.";
            int id = 1;
            int expectedStatusCode = 500;
            DeleteCinemaDomainModel cinemaModel = _deletedCinemaError;
            Task<DeleteCinemaDomainModel> responseTask = Task.FromResult(cinemaModel);


            _cinemaService.Setup(x => x.DeleteCinema(id)).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object);

            //Act
            var result = cinemasController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (ObjectResult)result;
            var badObjectResult = ((ObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

    }
}

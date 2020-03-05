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
    public class CinemaControllerTests
    {
        private Mock<ICinemaService> _cinemaService;

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
            Assert.AreEqual(cinemaDomainModel.Id, cinemaDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
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

       
    }
}

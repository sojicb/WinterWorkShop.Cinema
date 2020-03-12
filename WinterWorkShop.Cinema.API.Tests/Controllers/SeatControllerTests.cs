using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class SeatControllerTests
    {
        private Mock<ISeatService> _seatService;
        private Mock<ISeatReservationService> _seatReservationService = new Mock<ISeatReservationService>();

        [TestMethod]
        public void GetAsync_Return_All_Seats()
        {
            //Arrange
            List<SeatDomainModel> seatDomainModelsList = new List<SeatDomainModel>();
            SeatDomainModel seatDomainModel = new SeatDomainModel
            {
                Id = Guid.NewGuid(),
                AuditoriumId = 1,
                Row = 2,
                Number = 2
            };

            seatDomainModelsList.Add(seatDomainModel);
            IEnumerable<SeatDomainModel> seatDomainModels = seatDomainModelsList;
            Task<IEnumerable<SeatDomainModel>> responseTask = Task.FromResult(seatDomainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _seatService = new Mock<ISeatService>();
            _seatService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            SeatsController seatsController = new SeatsController(_seatService.Object, _seatReservationService.Object);

            //Act
            var result = seatsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var seatDomainModelResultList = (List<SeatDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(seatDomainModelResultList);
            Assert.AreEqual(expectedResultCount, seatDomainModelResultList.Count);
            Assert.AreEqual(seatDomainModel.Id, seatDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);

        }

        [TestMethod]
        public void GetAsync_Return_NewList()
        {
            //Arrange
            IEnumerable<SeatDomainModel> seatDomainModels = null;
            Task<IEnumerable<SeatDomainModel>> responseTask = Task.FromResult(seatDomainModels);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _seatService = new Mock<ISeatService>();
            _seatService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            SeatsController seatsController = new SeatsController(_seatService.Object, _seatReservationService.Object);

            //Act
            var result = seatsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var seatDomainModelResultList = (List<SeatDomainModel>)resultList;

            //Assert    
            Assert.IsNotNull(seatDomainModelResultList);
            Assert.AreEqual(expectedResultCount, seatDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

    }
}

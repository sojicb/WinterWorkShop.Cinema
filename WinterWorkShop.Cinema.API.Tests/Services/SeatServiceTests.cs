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
    public class SeatServiceTests
    {
        private Mock<ISeatsRepository> _mockSeatsRepository = new Mock<ISeatsRepository>();
        private Seat _seat;
        private SeatDomainModel _seatDomainModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _seat = new Seat
            {
                Id = Guid.NewGuid(),
                AuditoriumId = 1,
                Number = 1,
                Row = 1,
                
            };

            _seatDomainModel = new SeatDomainModel
            {
                Id = Guid.NewGuid(),
                AuditoriumId = 1,
                Row = 1,
                Number = 1
            };
            
        }

        [TestMethod]
        public void SeatService_GetAllAsync_ReturnSeats()
        {
            //Arrange
            int expectedResult = 1;
            List<Seat> seatsModelList = new List<Seat>();
            seatsModelList.Add(_seat);
            IEnumerable<Seat> seats = seatsModelList;
            Task<IEnumerable<Seat>> responseTask = Task.FromResult(seats);

            _mockSeatsRepository.Setup(x => x.GetAll()).Returns(responseTask);

            SeatService seatService = new SeatService(_mockSeatsRepository.Object);

            //Act
            var resultAction = seatService.GetAllAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()
                .ToList();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedResult, resultAction.Count);
        }

        [TestMethod]
        public void SeatService_GetAllAsync_InsertNull_ReturnNull()
        {
            //Arrange
            IEnumerable<Seat> seats = null;
            Task<IEnumerable<Seat>> responseTask = Task.FromResult(seats);

            _mockSeatsRepository.Setup(x => x.GetAll()).Returns(responseTask);

            SeatService seatService = new SeatService(_mockSeatsRepository.Object);

            //Act
            var resultAction = seatService.GetAllAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

    }
}

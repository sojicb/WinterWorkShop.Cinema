using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class SeatServiceTests
    {
        private Mock<ISeatRepository> _mockSeatsRepository;
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

            List<Seat> seatsModelList = new List<Seat>();
            
            seatsModelList.Add(_seat);
            IEnumerable<Seat> seats = seatsModelList;
            Task<IEnumerable<Seat>> responseTask = Task.FromResult(seats);

            _mockSeatsRepository = new Mock<ISeatRepository>();
            _mockSeatsRepository.Setup(x => x.GetAll()).Returns(responseTask);
        }

       
    }
}

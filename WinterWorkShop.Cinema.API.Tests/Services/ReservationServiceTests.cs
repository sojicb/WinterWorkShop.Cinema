using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
	[TestClass]
	public class ReservationServiceTests
	{
		private Mock<IReservationRepository> _reservationRepository;
		private Mock<IProjectionService> _projectionService;
		private Mock<ISeatReservationService> _seatReservationService;
		private Mock<ISeatService> _seatService;
		private Reservation _reservation;
		private Projection _projection;
		private ReservationDomainModel _reservationDomain;
		private CreateReservationResultModel _createReservation;
		private SeatReservationDomainModel _seat;

		[TestInitialize]
		public void TestInitializre()
		{

			_projection = new Projection()
			{
				Id = Guid.NewGuid(),
				DateTime = DateTime.Parse("2021-02-28 23:00:00.000"),
				MovieId = Guid.NewGuid(),
				AuditoriumId = 3
			};

			_reservation = new Reservation
			{
				Id = Guid.NewGuid(),
				ProjectionId = Guid.NewGuid(),
				UserId = Guid.NewGuid()
			};

			_reservationDomain = new ReservationDomainModel
			{
				Id = Guid.NewGuid(),
				ProjectionId = Guid.NewGuid(),
				UserId = Guid.NewGuid(),
				SeatIds = new List<Guid>
				{
					new Guid(),
					new Guid()
				}
			};

			_seat = new SeatReservationDomainModel
			{
				ProjectionTime = DateTime.Parse("2021-02-28 23:00:00.000"),
				Reservation = _reservationDomain,
			};

			_createReservation = new CreateReservationResultModel
			{
				ErrorMessage = null,
				IsSuccessful = true,
				Reservation = _reservationDomain
			};
		}

		[TestMethod]
		public void ReservationService_GetAllAsync_ReturnsReservations()
		{
			//Arrange
			_reservationRepository = new Mock<IReservationRepository>();
			_projectionService = new Mock<IProjectionService>();
			_seatReservationService = new Mock<ISeatReservationService>();
			_seatService = new Mock<ISeatService>();
			int expectedResult = 1;
			List<Reservation> reservations = new List<Reservation>();
			reservations.Add(_reservation);
			IEnumerable<Reservation> allReservations = reservations;
			Task<IEnumerable<Reservation>> responseTask = Task.FromResult(allReservations);

			_reservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
			ReservationService reservationService = new ReservationService(_reservationRepository.Object, _projectionService.Object, _seatReservationService.Object, _seatService.Object);

			//Act
			var resultAction = reservationService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult().ToList();

			//Assert
			Assert.IsNotNull(resultAction);
			Assert.AreEqual(resultAction.Count, expectedResult);
		}

		[TestMethod]
		public void ReservationService_GetAllAsync_InsertedNull_ReturnsNull()
		{
			//Arrange
			_reservationRepository = new Mock<IReservationRepository>();
			_projectionService = new Mock<IProjectionService>();
			_seatReservationService = new Mock<ISeatReservationService>();
			_seatService = new Mock<ISeatService>();
			
			IEnumerable<Reservation> allReservations = null;
			Task<IEnumerable<Reservation>> responseTask = Task.FromResult(allReservations);

			_reservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
			ReservationService reservationService = new ReservationService(_reservationRepository.Object, _projectionService.Object, _seatReservationService.Object, _seatService.Object);

			//Act
			var resultAction = reservationService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsNull(resultAction);
		}

		[TestMethod]
		public void ReservationService_CreateReservation_ReturnReservation()
		{
			//Arrange
			_reservationRepository = new Mock<IReservationRepository>();
			_projectionService = new Mock<IProjectionService>();
			_seatReservationService = new Mock<ISeatReservationService>();
			_seatService = new Mock<ISeatService>();
			List<SeatReservationDomainModel> seatReservationDomainModels = new List<SeatReservationDomainModel>();
			seatReservationDomainModels.Add(_seat);
			IEnumerable<SeatReservationDomainModel> seatReservation = seatReservationDomainModels;
			Task<IEnumerable<SeatReservationDomainModel>> response = Task.FromResult(seatReservation);

			Reservation reservation = _reservation;
			Task<Reservation> responseTask = Task.FromResult(reservation);

			_reservationRepository.Setup(x => x.Insert(reservation)).Returns(responseTask.Result);
			_seatReservationService.Setup(x => x.InsertResevedSeats(It.IsAny<InsertSeatReservationModel>())).Returns(response);
			ReservationService reservationService = new ReservationService(_reservationRepository.Object, _projectionService.Object, _seatReservationService.Object, _seatService.Object);
			//SeatReservationService seatReservationService = new SeatReservationService(_seatReservationService, _)
			//Act
			var resultAction = reservationService.CreateReservation(_reservationDomain).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsNotNull(resultAction);
			Assert.IsNull(resultAction.ErrorMessage);
			Assert.IsTrue(resultAction.IsSuccessful);
		}

		[TestMethod]
		public void ReservationService_CreateReservation_InsertedNull_ReturnReservationCreationError()
		{
			//Arrange
			_reservationRepository = new Mock<IReservationRepository>();
			_projectionService = new Mock<IProjectionService>();
			_seatReservationService = new Mock<ISeatReservationService>();
			_seatService = new Mock<ISeatService>();

			ReservationDomainModel reservationDomain = null;
			Reservation reservation = null;
			Task<Reservation> responseTask = Task.FromResult(reservation);

			_reservationRepository.Setup(x => x.Insert(It.IsAny<Reservation>())).Returns(responseTask.Result);
			ReservationService reservationService = new ReservationService(_reservationRepository.Object, _projectionService.Object, _seatReservationService.Object, _seatService.Object);

			//Act
			var resultAction = reservationService.CreateReservation(reservationDomain).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsFalse(resultAction.IsSuccessful);
		}

		[TestMethod]
		public void ReservationService_CreateReservation_InsertedWrongSeatScheme_ReturnSeatReservationError() {
			//Arrange
			_reservationRepository = new Mock<IReservationRepository>();
			_projectionService = new Mock<IProjectionService>();
			_seatReservationService = new Mock<ISeatReservationService>();
			_seatService = new Mock<ISeatService>();

			IEnumerable<Reservation> allReservations = null;
			Task<IEnumerable<Reservation>> responseTask = Task.FromResult(allReservations);

			_reservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
			ReservationService reservationService = new ReservationService(_reservationRepository.Object, _projectionService.Object, _seatReservationService.Object, _seatService.Object);

			//Act
			var resultAction = reservationService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsNull(resultAction);
		}
	}
}

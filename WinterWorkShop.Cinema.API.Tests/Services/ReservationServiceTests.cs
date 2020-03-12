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
		private Mock<IReservationRepository> _reservationRepository = new Mock<IReservationRepository>();
		private Mock<IProjectionService> _projectionService = new Mock<IProjectionService>();
		private Mock<ISeatReservationService> _seatReservationService = new Mock<ISeatReservationService>();
		private Mock<ISeatService> _seatService = new Mock<ISeatService>();
		private Reservation _reservation;
		private Projection _projection;
		private ReservationDomainModel _reservationDomain;
		private CreateReservationResultModel _createReservation;
		private SeatReservationDomainModel _seat;
		private ValidateSeatDomainModel _seatValidation;
		private SeatValidationDomainModel _seatValidationDomain;

		[TestInitialize]
		public void TestInitializre()
		{
			_seatValidationDomain = new SeatValidationDomainModel
			{
				AuditoriumId = 1,
				ProjectionTime = DateTime.Parse("2021-02-28 23:00:00.000"),
				SeatIds = new List<Guid>
				{
					new Guid()
				}
			};

			_seatValidation = new ValidateSeatDomainModel
			{
				ErrorMessage = null,
				IsSuccessful = true
			};

			_projection = new Projection()
			{
				Id = Guid.NewGuid(),
				DateTime = DateTime.Parse("2021-02-28 23:00:00.000"),
				MovieId = Guid.NewGuid(),
				AuditoriumId = 3
			};

			SeatReservation seatReservation = new SeatReservation
			{
				Seat = new Seat
				{
					Id = new Guid(),
					AuditoriumId = 1,
					Number = 1,
					Row = 1
				}
			};

			_reservation = new Reservation
			{
				Id = new Guid("99B00556-D669-444B-37DE-08D7C50563B8"),
				ProjectionId = new Guid("1827EE4B-56FA-4D50-5B51-08D7C40EEB04"),
				UserId = new Guid("206F083A-1080-4EA3-92E4-62105C33FCB9"),
				SeatReservation = new List<SeatReservation> { seatReservation}
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
		public void ReservationService_GetAllAsync_InsertedNull_ReturnsNull()
		{
			//Arrange
			IEnumerable<Reservation> allReservations = null;
			Task<IEnumerable<Reservation>> responseTask = Task.FromResult(allReservations);

			_reservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
			ReservationService reservationService = new ReservationService(_reservationRepository.Object, _projectionService.Object, _seatReservationService.Object, _seatService.Object);

			//Act
			var resultAction = reservationService.GetAllAsync()
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();

			//Assert
			Assert.IsNull(resultAction);
		}

		[TestMethod]
		public void ReservationService_CreateReservation_ReturnReservation()
		{
			//Arrange
			List<SeatReservationDomainModel> seatReservationDomainModels = new List<SeatReservationDomainModel>();
			seatReservationDomainModels.Add(_seat);
			IEnumerable<SeatReservationDomainModel> seatReservation = seatReservationDomainModels;
			Task<IEnumerable<SeatReservationDomainModel>> response = Task.FromResult(seatReservation);

			Reservation reservation = _reservation;
			Task<Reservation> responseTask = Task.FromResult(reservation);


			_reservationRepository.Setup(x => x.Insert(It.IsAny<Reservation>())).Returns(responseTask.Result);
			_seatReservationService.Setup(x => x.InsertResevedSeats(It.IsAny<InsertSeatReservationModel>())).Returns(response);
			ReservationService reservationService = new ReservationService(_reservationRepository.Object, _projectionService.Object, _seatReservationService.Object, _seatService.Object);

			//Act
			var resultAction = reservationService.CreateReservation(_reservationDomain)
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();

			//Assert
			Assert.IsNotNull(resultAction);
			Assert.IsNull(resultAction.ErrorMessage);
			Assert.IsTrue(resultAction.IsSuccessful);
		}

		[TestMethod]
		public void ReservationService_CreateReservation_InsertedNull_ReturnReservationCreationError()
		{
			//Arrange
			string message = "Error occurred while creating new reservation, please try again.";
			Reservation reservation = null;
			Task<Reservation> responseTask = Task.FromResult(reservation);

			_reservationRepository.Setup(x => x.Insert(It.IsAny<Reservation>())).Returns(responseTask.Result);
			ReservationService reservationService = new ReservationService(_reservationRepository.Object, _projectionService.Object, _seatReservationService.Object, _seatService.Object);

			//Act
			var resultAction = reservationService
				.CreateReservation(_reservationDomain)
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();

			//Assert
			Assert.IsFalse(resultAction.IsSuccessful);
			Assert.AreEqual(resultAction.ErrorMessage, message);
		}

		[TestMethod]
		public void ReservationService_CreateReservation_InsertedWrongSeatScheme_ReturnSeatReservationError() {

			//Arrange
			string message = "Error occurred while reserving seats, please try again.";
			Reservation allReservations = _reservation;
			Task<Reservation> responseTask = Task.FromResult(allReservations);

			IEnumerable<SeatReservationDomainModel> insertSeats = null;
			Task<IEnumerable<SeatReservationDomainModel>> response = Task.FromResult(insertSeats);

			_reservationRepository.Setup(x => x.Insert(It.IsAny<Reservation>())).Returns(responseTask.Result);
			_seatReservationService.Setup(x => x.InsertResevedSeats(It.IsAny<InsertSeatReservationModel>())).Returns(response);
			ReservationService reservationService = new ReservationService(_reservationRepository.Object, _projectionService.Object, _seatReservationService.Object, _seatService.Object);

			//Act
			var resultAction = reservationService.CreateReservation(_reservationDomain).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsFalse(resultAction.IsSuccessful);
			Assert.AreEqual(resultAction.ErrorMessage, message);
		}

		[TestMethod]
		public void ReservationService_HandleSeatValidation_InsertedWrongSeatScheme_ReturnErrorMessage()
		{

			//Arrange
			string message = "Error occurred while reserving seats, please try again.";
			Reservation allReservations = _reservation;
			Task<Reservation> responseTask = Task.FromResult(allReservations);

			IEnumerable<SeatReservationDomainModel> insertSeats = null;
			Task<IEnumerable<SeatReservationDomainModel>> response = Task.FromResult(insertSeats);

			_reservationRepository.Setup(x => x.Insert(It.IsAny<Reservation>())).Returns(responseTask.Result);
			_seatReservationService.Setup(x => x.InsertResevedSeats(It.IsAny<InsertSeatReservationModel>())).Returns(response);
			ReservationService reservationService = new ReservationService(_reservationRepository.Object, _projectionService.Object, _seatReservationService.Object, _seatService.Object);

			//Act
			var resultAction = reservationService.CreateReservation(_reservationDomain).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsFalse(resultAction.IsSuccessful);
			Assert.AreEqual(resultAction.ErrorMessage, message);
		}
	}
}

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
	public class SeatReservationServiceTests
	{
		private  Mock<ISeatReservationRepository> _mockSeatReservationRepository = new Mock<ISeatReservationRepository>();
		private  Mock<ISeatsRepository> _mockSeatRepository = new Mock<ISeatsRepository>();
		private  Mock<ISeatReservationService> _mockSeatReservationService = new Mock<ISeatReservationService>();

		private Seat _seatOne;
		private Seat _seatTwo;
		private Seat _seatThree;
		private Seat _seatFour;
		private Reservation _reservation;
		private SeatReservation _seatReservation;
		private SeatReservationDomainModel _seatReservationDomain;
		private SeatReservationDomainModel _seatReservationDomainTwo;
		private InsertSeatReservationModel _insertSeat;
		private SeatValidationDomainModel _seatValidation;
		private SeatReservationValidationDomainModel _validationDomainModel;

		[TestInitialize]
		public void TestInitialize()
		{

			_validationDomainModel = new SeatReservationValidationDomainModel
			{
				ErrorMessage = null,
				IsSuccessful = true
			};

			_seatValidation = new SeatValidationDomainModel
			{
				AuditoriumId = 1,
				ProjectionTime = DateTime.Parse("2022-09-20 00:07:57.590"),
				SeatIds = new List<Guid>
				{
					new Guid("5E910A02-C5AD-4B42-63A5-08D7C40EE083"),
				}
			};

			_seatTwo = new Seat
			{
				Id = new Guid("2C5E0E8F-ADA8-47CC-63A6-08D7C40EE083"),
				Number = 2,
				Row = 1
			};

			_seatThree = new Seat
			{
				Id = new Guid("CE630486-1C44-4FFD-63A7-08D7C40EE083"),
				Number = 3,
				Row = 1
			};

			_seatOne = new Seat
			{
				Id = new Guid("5E910A02-C5AD-4B42-63A5-08D7C40EE083"),
				Number = 1,
				Row = 1
			};

			_seatReservation = new SeatReservation
			{
				ProjectionTime = DateTime.Parse("2022-09-20 00:07:57.590"),
				Seat = _seatOne,
				SeatId = new Guid("5E910A02-C5AD-4B42-63A5-08D7C40EE083"),
				ReservationId = new Guid()
			};

			_reservation = new Reservation
			{
				Id = new Guid(),
				UserId = new Guid(),
				ProjectionId = new Guid()
			};

			_seatReservationDomain = new SeatReservationDomainModel
			{
				ProjectionTime = DateTime.Parse("2022-09-20 00:07:57.590"),
				ReservationId = new Guid(),
				SeatId = _seatOne.Id
			};

			_insertSeat = new InsertSeatReservationModel
			{
				ReservationId = new Guid("5EF71187-4AB0-46BA-8B11-08D7C594CD5F"),
				ProjectionTime = DateTime.Parse("2022-09-20 00:07:57.590"),
				SeatIds = new List<Guid>
				{
					new Guid("5E910A02-C5AD-4B42-63A5-08D7C40EE083"),
				}
			};
			_seatReservationDomainTwo = new SeatReservationDomainModel
			{
				ProjectionTime = DateTime.Parse("2022-09-20 00:07:57.590"),
				ReservationId = new Guid(),
				SeatId = new Guid("925494B7-FBE6-4A31-63A8-08D7C40EE083")
			};
		}

		[TestMethod]
		public void SeatReservationService_GetAllAsync_ReturnSeatReservations()
		{
			//Arrange
			int expectedResult = 1;
			List<SeatReservation> mockedSeatReservations = new List<SeatReservation>();
			mockedSeatReservations.Add(_seatReservation);
			IEnumerable<SeatReservation> seatReservations = mockedSeatReservations;
			Task<IEnumerable<SeatReservation>> responseTask = Task.FromResult(seatReservations);

			_mockSeatReservationRepository.Setup(x => x.GetAll()).Returns(responseTask);

			SeatReservationService reservationService = new SeatReservationService(_mockSeatReservationRepository.Object, _mockSeatRepository.Object);

			//Act
			var resultAction = reservationService.GetAllAsync()
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult()
				.ToList();

			//Assert
			Assert.IsNotNull(resultAction);
			Assert.AreEqual(resultAction.Count, expectedResult);
		}

		[TestMethod]
		public void SeatReservationService_GetAllAsync_InsertedMockedNull_ReturnsNull()
		{
			//Arrange
			IEnumerable<SeatReservation> seatReservations = null;
			Task<IEnumerable<SeatReservation>> responseTask = Task.FromResult(seatReservations);

			_mockSeatReservationRepository.Setup(x => x.GetAll()).Returns(responseTask);

			SeatReservationService reservationService = new SeatReservationService(_mockSeatReservationRepository.Object, _mockSeatRepository.Object);

			//Act
			var resultAction = reservationService.GetAllAsync()
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();

			//Assert
			Assert.IsNull(resultAction);
		}

		[TestMethod]
		public void SeatReservationService_InsertReservedSeats_ReturnInsertedSeatReservations()
		{
			//Arrange
			int expectedResult = 1;
			SeatReservation seatReservations = _seatReservation;
			Task<SeatReservation> responseTask = Task.FromResult(seatReservations);

			_mockSeatReservationRepository.Setup(x => x.Insert(It.IsAny<SeatReservation>())).Returns(responseTask.Result);

			SeatReservationService reservationService = new SeatReservationService(_mockSeatReservationRepository.Object, _mockSeatRepository.Object);

			//Act
			var resultAction = reservationService.InsertResevedSeats(_insertSeat)
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult()
				.ToList();

			//Assert
			Assert.IsNotNull(resultAction);
			Assert.AreEqual(resultAction.Count, expectedResult);
		}

		[TestMethod]
		public void SeatReservationService_InsertReservedSeats_InsertedNull_ReturnsNull()
		{
			//Arrange
			SeatReservation seatReservation = null;
			Task<SeatReservation> responseTask = Task.FromResult(seatReservation);

			_mockSeatReservationRepository.Setup(x => x.Insert(It.IsAny<SeatReservation>())).Returns(responseTask.Result);

			SeatReservationService reservationService = new SeatReservationService(_mockSeatReservationRepository.Object, _mockSeatRepository.Object);

			//Act
			var resultAction = reservationService.InsertResevedSeats(_insertSeat)
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();

			//Assert
			Assert.IsNull(resultAction);
		}

		[TestMethod]
		public void SeatReservationService_HandleSeatReservation_ReturnErrorSeatsReserved()
		{
			//Arrange
			string expectedMessage = "This seat is already reserved, please choose another.";
			List<SeatReservation> mockedSeatReservations = new List<SeatReservation>();
			mockedSeatReservations.Add(_seatReservation);
			IEnumerable<SeatReservation> seatReservations = mockedSeatReservations;
			Task<IEnumerable<SeatReservation>> responseTask = Task.FromResult(seatReservations);

			SeatReservationValidationDomainModel seat = _validationDomainModel;
			Task<SeatReservationValidationDomainModel> response = Task.FromResult(seat);

			_mockSeatReservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
			_mockSeatReservationService.Setup(x => x.ValidateSeat(_seatReservationDomainTwo)).Returns(response);


			SeatReservationService reservationService = new SeatReservationService(_mockSeatReservationRepository.Object, _mockSeatRepository.Object);

			//Act
			var resultAction = reservationService.HandleSeatReservation(_seatValidation)
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();

			//Assert
			Assert.IsNotNull(resultAction);
			Assert.AreEqual(resultAction.ErrorMessage, expectedMessage);
		}

		[TestMethod]
		public void SeatReservationService_GetReservedSeats_InsertMockedNull_ReturnsNull()
		{
			//Arrange
			int expectedResult = 1;
			List<SeatReservation> mockedSeatReservations = new List<SeatReservation>();
			mockedSeatReservations.Add(_seatReservation);
			IEnumerable<SeatReservation> seatReservations = mockedSeatReservations;
			Task<IEnumerable<SeatReservation>> responseTask = Task.FromResult(seatReservations);

			List<Seat> mockedSeats = new List<Seat>();
			mockedSeats.Add(_seatOne);
			IEnumerable<Seat> seats = mockedSeats;
			Task<IEnumerable<Seat>> response = Task.FromResult(seats);


			_mockSeatReservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
			_mockSeatRepository.Setup(x => x.GetSeatsByAuditoriumId(1)).Returns(response);


			SeatReservationService reservationService = new SeatReservationService(_mockSeatReservationRepository.Object, _mockSeatRepository.Object);

			//Act
			var resultAction = reservationService.GetReservedSeats(1, DateTime.Parse("2022-09-20 00:07:57.590"))
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult().ToList() ;

			//Assert
			Assert.IsNotNull(resultAction);
			Assert.AreEqual(resultAction.Count, expectedResult);
		}

		[TestMethod]
		public void SeatReservationService_GetReservedSeats_ReturnsReservedSeats()
		{
			//Arrange
			int expectedResult = 1;
			List<SeatReservation> mockedSeatReservations = new List<SeatReservation>();
			mockedSeatReservations.Add(_seatReservation);
			IEnumerable<SeatReservation> seatReservations = mockedSeatReservations;
			Task<IEnumerable<SeatReservation>> responseTask = Task.FromResult(seatReservations);

			List<Seat> mockedSeats = new List<Seat>();
			mockedSeats.Add(_seatOne);
			IEnumerable<Seat> seats = mockedSeats;
			Task<IEnumerable<Seat>> response = Task.FromResult(seats);


			_mockSeatReservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
			_mockSeatRepository.Setup(x => x.GetSeatsByAuditoriumId(1)).Returns(response);


			SeatReservationService reservationService = new SeatReservationService(_mockSeatReservationRepository.Object, _mockSeatRepository.Object);

			//Act
			var resultAction = reservationService.GetReservedSeats(1, DateTime.Parse("2022-09-20 00:07:57.590"))
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult().ToList();

			//Assert
			Assert.IsNotNull(resultAction);
			Assert.AreEqual(resultAction.Count, expectedResult);
		}

		[TestMethod]
		public void SeatReservationService_GetReservedSeats_InsertAllSeatsNull_ReturnsNull()
		{
			//Arrange
			List<SeatReservation> mockedSeatReservations = new List<SeatReservation>();
			mockedSeatReservations.Add(_seatReservation);
			IEnumerable<SeatReservation> seatReservations = mockedSeatReservations;
			Task<IEnumerable<SeatReservation>> responseTask = Task.FromResult(seatReservations);

			List<Seat> mockedSeats = new List<Seat>();
			mockedSeats.Add(_seatOne);
			IEnumerable<Seat> seats = mockedSeats;
			Task<IEnumerable<Seat>> response = Task.FromResult(seats);


			_mockSeatReservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
			_mockSeatRepository.Setup(x => x.GetSeatsByAuditoriumId(1)).Returns(response);


			SeatReservationService reservationService = new SeatReservationService(_mockSeatReservationRepository.Object, _mockSeatRepository.Object);

			//Act
			var resultAction = reservationService.GetReservedSeats(1, DateTime.Parse("2012-09-20 00:07:57.590"))
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();

			//Assert
			Assert.IsNull(resultAction);
		}

		[TestMethod]
		public void SeatReservationService_GetReservedSeats_InsertAuditoriumInvalidIdNull_ReturnsNull()
		{
			//Arrange
			List<SeatReservation> mockedSeatReservations = new List<SeatReservation>();
			mockedSeatReservations.Add(_seatReservation);
			IEnumerable<SeatReservation> seatReservations = mockedSeatReservations;
			Task<IEnumerable<SeatReservation>> responseTask = Task.FromResult(seatReservations);

			List<Seat> mockedSeats = new List<Seat>();
			mockedSeats.Add(_seatOne);
			IEnumerable<Seat> seats = mockedSeats;
			Task<IEnumerable<Seat>> response = Task.FromResult(seats);


			_mockSeatReservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
			_mockSeatRepository.Setup(x => x.GetSeatsByAuditoriumId(2)).Returns(response);


			SeatReservationService reservationService = new SeatReservationService(_mockSeatReservationRepository.Object, _mockSeatRepository.Object);

			//Act
			var resultAction = reservationService.GetReservedSeats(1, DateTime.Parse("2012-09-20 00:07:57.590"))
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();

			//Assert
			Assert.IsNull(resultAction);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
	public class ReservationService : IReservationService
	{

		private readonly IReservationRepository _reservationRepository;
		private readonly IProjectionService _projectionService;
		private readonly ISeatReservationService _seatReservationService;
		private readonly ISeatService _seatService;

		public ReservationService(IReservationRepository reservationRepository, IProjectionService projectionService, ISeatReservationService seatReservationService, ISeatService seatService)
		{
			_reservationRepository = reservationRepository;
			_projectionService = projectionService;
			_seatReservationService = seatReservationService;
			_seatService = seatService;
		}

		public async Task<CreateReservationResultModel> CreateReservation(ReservationDomainModel domainModel)
		{
			Reservation reservation = new Reservation 
			{ 
				ProjectionId = domainModel.ProjectionId,
				UserId = domainModel.UserId
			};

			var data = _reservationRepository.Insert(reservation);

			if(data == null)
			{
				return new CreateReservationResultModel
				{
					IsSuccessful = false,
					ErrorMessage = Messages.RESERVATION_CREATION_ERROR
				};
			}

			InsertSeatReservationModel model = new InsertSeatReservationModel
			{
				ProjectionTime = domainModel.ProjectionTime,
				ReservationId = data.Id,
				SeatIds = domainModel.SeatIds
			};

			var seats = await _seatReservationService.InsertResevedSeats(model);

			if (seats == null)
			{
				return new CreateReservationResultModel
				{
					IsSuccessful = false,
					ErrorMessage = Messages.SEAT_RESERVATION_ERROR
				};
			}

			_reservationRepository.Save();

			CreateReservationResultModel reservationDomain = new CreateReservationResultModel()
			{
				IsSuccessful = true,
				ErrorMessage = null,
				Reservation = new ReservationDomainModel
				{
					Id = data.Id,
					ProjectionId = data.ProjectionId,
					UserId = data.UserId
				}
			};

			return reservationDomain;
		}

		public async Task<IEnumerable<ReservationDomainModel>> GetAllAsync()
		{
			var data = await _reservationRepository.GetAll();

			if(data == null)
			{
				return null;
			}

			List<ReservationDomainModel> reservations = new List<ReservationDomainModel>();
			List<SeatDomainModel> reservedSeats = new List<SeatDomainModel>();

			foreach (var reservation in data)
			{
				var seatIds = _seatReservationService.GetAllAsync().Result.Where(x => x.ReservationId.Equals(reservation.Id)).Select(y => y.SeatId).ToList();
				var projection = await _projectionService.GetProjectionByIdAsync(reservation.ProjectionId);
				var allSeats = _seatService.GetAllAsync().Result.ToList();
				var seats = allSeats.Where(y => seatIds.Contains(y.Id)).ToList();

				foreach (var seat in seats)
				{
					reservedSeats.Add(new SeatDomainModel
					{
					Id = seat.Id,
					Number = seat.Number,
					Row = seat.Row
					});
				}

				reservations.Add(new ReservationDomainModel
				{
					Id = reservation.Id,
					MovieTitle = projection.MovieTitle,
					ProjectionId = reservation.ProjectionId,
					Username = reservation.User.UserName,
					ProjectionTime = reservation.Projection.DateTime,
					SeatIds = seatIds
				});
				reservedSeats = new List<SeatDomainModel>();
			}

			return reservations;
		}

		public async Task<ValidateSeatDomainModel> HandleSeatValidation(ReservationDomainModel reservationDomain)
		{
			SeatValidationDomainModel model = new SeatValidationDomainModel
			{
				AuditoriumId = reservationDomain.AuditoriumId,
				ProjectionTime = reservationDomain.ProjectionTime,
				SeatIds = reservationDomain.SeatIds
			};

			var data = await _seatReservationService.HandleSeatReservation(model);

			if (!data.IsSuccessful)
			{
				return new ValidateSeatDomainModel
					{
						ErrorMessage = data.ErrorMessage,
						IsSuccessful = false,
						Seat = data.Seat
					};
			}

			return new ValidateSeatDomainModel
			{
				IsSuccessful = true,
				ErrorMessage = null
			};
		}
	}
}

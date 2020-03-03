using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public Task<ReservationDomainModel> CreateReservation(AuditoriumDomainModel domainModel)
		{
			throw new NotImplementedException();
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
					Seats = reservedSeats
				});
				reservedSeats = new List<SeatDomainModel>();
			}

			return reservations;
		}

		public Task<ReservationDomainModel> GetByIdAsync(Guid id)
		{
			throw new NotImplementedException();
		}
	}
}

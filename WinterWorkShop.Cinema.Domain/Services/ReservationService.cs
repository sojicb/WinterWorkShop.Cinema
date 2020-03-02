using System;
using System.Collections.Generic;
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

		public ReservationService(IReservationRepository reservationRepository, IProjectionService projectionService)
		{
			_reservationRepository = reservationRepository;
			_projectionService = projectionService;
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
			List<SeatDomainModel> seats = new List<SeatDomainModel>();

			foreach(var reservation in data)
			{
				var projection = await _projectionService.GetProjectionByIdAsync(reservation.ProjectionId);
				reservations.Add(new ReservationDomainModel
				{
					Id = reservation.Id,
					MovieTitle = projection.MovieTitle,
					ProjectionId = reservation.ProjectionId,
					Username = reservation.User.UserName,
					ProjectionTime = reservation.Projection.DateTime,
				});
			}

			return reservations;
		}

		public Task<ReservationDomainModel> GetByIdAsync(Guid id)
		{
			throw new NotImplementedException();
		}
	}
}

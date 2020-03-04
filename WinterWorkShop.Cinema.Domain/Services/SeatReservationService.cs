using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
	public class SeatReservationService : ISeatReservationService
	{
		private readonly ISeatReservationRepository _seatReservationRepository;

		public SeatReservationService(ISeatReservationRepository seatReservationRepository)
		{
			_seatReservationRepository = seatReservationRepository;
		}

		public async Task<IEnumerable<SeatReservationDomainModel>> GetAllAsync()
		{
			var data = await _seatReservationRepository.GetAll();

			if(data == null)
			{
				return null;
			}

			List<SeatReservationDomainModel> seatReservations = new List<SeatReservationDomainModel>();

			foreach(var seatReservation in data)
			{
				seatReservations.Add(new SeatReservationDomainModel
				{
					ReservationId = seatReservation.ReservationId,
					SeatId = seatReservation.SeatId,
					Seat = new SeatDomainModel
					{
						Id = seatReservation.Seat.Id,
						AuditoriumId = seatReservation.Seat.AuditoriumId,
						Number = seatReservation.Seat.Number,
						Row = seatReservation.Seat.Row
					},
					Reservation = new ReservationDomainModel
					{
						Id = seatReservation.ReservationId
					}
				});
			}

			return seatReservations;
		}


	}
}

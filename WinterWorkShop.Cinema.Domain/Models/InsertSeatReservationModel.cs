using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
	public class InsertSeatReservationModel
	{
		public Guid ReservationId { get; set; }
		public DateTime ProjectionTime { get; set; }
		public List<SeatDomainModel> Seats { get; set; }
		public ReservationDomainModel Reservation { get; set; }
	}
}

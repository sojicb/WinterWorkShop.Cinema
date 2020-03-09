using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
	public class SeatReservationDomainModel
	{
		public Guid ReservationId { get; set; }
		public Guid SeatId { get; set; }
		public DateTime ProjectionTime { get; set; }
		public SeatDomainModel Seat { get; set; }
		public ReservationDomainModel Reservation { get; set; }
	}
}

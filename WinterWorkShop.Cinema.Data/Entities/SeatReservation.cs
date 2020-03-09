using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data.Entities
{
	[Table("SeatReservation")]
	public class SeatReservation
	{
		[Key, Column("reservationId", Order = 1)]
		public Guid ReservationId { get; set; }

		[Key, Column("seatId", Order = 2)]
		public Guid SeatId { get; set; }

		[Column("projectionTime")]
		public DateTime ProjectionTime { get; set; }

		#region [Relationships]
		public virtual Reservation Reservation { get; set; }
		public virtual Seat Seat { get; set; }
		#endregion
	}
}

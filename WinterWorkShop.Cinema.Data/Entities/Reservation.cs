using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data.Entities
{
	[Table("Reservation")]
	public class Reservation
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid ProjectionId { get; set; }

		#region [Relationships]
		public virtual User User { get; set; }
		public virtual ICollection<SeatReservation> SeatReservation { get; set; }
		public virtual Projection Projection { get; set; }
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
	public class ReservationDomainModel
	{
		public Guid Id { get; set; }
		public string Username { get; set; }
		public Guid UserId { get; set; }
		public string MovieTitle { get; set; }
		public Guid ProjectionId { get; set; }
		public DateTime ProjectionTime { get; set; }
		public int AuditoriumId { get; set; }
		public List<Guid> SeatIds { get; set; }
	}
}

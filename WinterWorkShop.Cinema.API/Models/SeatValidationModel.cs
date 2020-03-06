using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Models
{
	public class SeatValidationModel
	{
		public List<SeatDomainModel> Seats { get; set; }
		public int AuditoriumId { get; set; }
		public DateTime ProjectionTime { get; set; }
	}
}

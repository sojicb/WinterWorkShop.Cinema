using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
	public class SeatReservationValidationDomainModel
	{
		public string ErrorMessage { get; set; }
		public bool IsSuccessful { get; set; }
		public SeatReservationDomainModel SeatReservation { get; set; }
	}
}

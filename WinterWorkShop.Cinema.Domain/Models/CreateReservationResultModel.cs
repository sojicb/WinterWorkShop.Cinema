using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
	public class CreateReservationResultModel
	{
		public string ErrorMessage { get; set; }
		public bool IsSuccessful { get; set; }
		public ReservationDomainModel Reservation { get; set; }
	}
}

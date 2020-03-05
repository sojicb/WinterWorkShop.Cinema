using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
	public class ValidateSeatDomainModel
	{
		public string ErrorMessage { get; set; }
		public bool IsSuccessful { get; set; }
		public SeatValidationDomainModel SeatValidation { get; set; }
	}
}

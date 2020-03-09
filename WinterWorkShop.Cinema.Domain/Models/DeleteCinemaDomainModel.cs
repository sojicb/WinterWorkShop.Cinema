using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
	public class DeleteCinemaDomainModel
	{
		public bool IsSuccessful { get; set; }

		public string ErrorMessage { get; set; }

		public CinemaDomainModel Cinema { get; set; }
	}
}

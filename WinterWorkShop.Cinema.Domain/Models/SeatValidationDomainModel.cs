﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
	public class SeatValidationDomainModel
	{
		public List<SeatDomainModel> Seats { get; set; }
		public int AuditoriumId { get; set; }
		public DateTime ProjectionTime { get; set; }
	}
}
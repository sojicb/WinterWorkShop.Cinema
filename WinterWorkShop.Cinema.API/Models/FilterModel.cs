﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Models
{
	public class FilterModel
	{
		public int? CinemaId { get; set; }
		public Guid? MovieId { get; set; }
		public int? AuditoriumId { get; set; }
		public DateTime? ProjectionDateFrom { get; set; }
		public DateTime? ProjectionDateTo { get; set; }
		public List<ProjectionDomainModel> Projections { get; set; }
	}
}
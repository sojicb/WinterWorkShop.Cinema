using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
	public class FilterDomainModel
	{
		public int? CinemaId { get; set; }
		public Guid? MovieId { get; set; }
		public int? AuditoriumId { get; set; }
		public DateTime? ProjectionDateFrom { get; set; }
		public DateTime? ProjectionDateTo { get; set; }
		public List<ProjectionDomainModel> Projections { get; set; }
	}
}

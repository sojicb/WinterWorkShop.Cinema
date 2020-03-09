using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
	public class MovieTagDomainModel
	{
		public Guid MovieId { get; set; }
		public int TagId { get; set; }
		public TagDomainModel Tag { get; set; }
		public MovieDomainModel Movie { get; set; }
	}
}

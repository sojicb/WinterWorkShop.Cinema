using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinterWorkShop.Cinema.Data.Entities
{
	[Table("MovieTags")]
	public class MovieTags
	{
		[Key, Column("movieId", Order = 1)]
		public Guid MovieId { get; set; }

		[Key, Column("tagId", Order = 2)]
		public int TagId { get; set; }

		#region [Relationships]
		public virtual Movie Movie { get; set; }
		public virtual Tag Tag { get; set; } 
		#endregion
	}
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinterWorkShop.Cinema.Data.Entities
{
	[Table("Tags")]
	public class Tag
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("specificValue")]
		public string Value { get; set; }

		#region [Relationships]
		public virtual ICollection<MovieTags> MovieTags { get; set; } 
		#endregion
	}
}

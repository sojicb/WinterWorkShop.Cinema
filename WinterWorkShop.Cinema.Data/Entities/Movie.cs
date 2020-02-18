using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data
{
    [Table("movie")]
    public class Movie
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public double? Rating { get; set; }

        public bool Current { get; set; }

        public virtual ICollection<Projection> Projections { get; set; }
    }
}

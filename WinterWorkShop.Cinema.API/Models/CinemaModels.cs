using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.API.Models
{
    public class CinemaModels
    { 
        [Required]
        [Range(1, 255)]
        public string Name { get; set; }
    }
}

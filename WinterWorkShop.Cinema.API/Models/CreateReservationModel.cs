using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Models
{
	public class CreateReservationModel
	{
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid ProjectionId { get; set; }
        [Required]
        public DateTime ProjectionTime { get; set; }
        [Required]
        public List<SeatDomainModel> Seats { get; set; }
    }
}

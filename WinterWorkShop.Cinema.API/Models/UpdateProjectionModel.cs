﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.API.Models
{
    public class UpdateProjectionModel
    {
        [Required]
        [Range(1, Int32.MaxValue)]
        public int AuditoriumId { get; set; }

        [Required]
        public DateTime ProjectionTime { get; set; }

        public Guid MovieId { get; set; }
    }
}

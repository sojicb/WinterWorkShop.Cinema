using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;
        private readonly ISeatReservationService _seatReservationService;

        public SeatsController(ISeatService seatService, ISeatReservationService seat)
        {
            _seatService = seatService;
            _seatReservationService = seat;
        }

        /// <summary>
        /// Gets all seats
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<SeatDomainModel>>> GetAsync()
        {
            IEnumerable<SeatDomainModel> seatDomainModels;
            
            seatDomainModels = await _seatService.GetAllAsync();

            if (seatDomainModels == null)
            {
                seatDomainModels = new List<SeatDomainModel>();
            }

            return Ok(seatDomainModels);
        }

        /// <summary>
        /// Gets all reserved seats
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("reserved")]
        public async Task<ActionResult<IEnumerable<SeatDomainModel>>> GetReservedSeats(int id, DateTime projectionTime)
        {
            IEnumerable<SeatDomainModel> seatDomainModels;

            seatDomainModels = await _seatReservationService.GetReservedSeats(id, projectionTime);

            if (seatDomainModels == null)
            {
                seatDomainModels = new List<SeatDomainModel>();
            }

            return Ok(seatDomainModels);
        }
    }
}

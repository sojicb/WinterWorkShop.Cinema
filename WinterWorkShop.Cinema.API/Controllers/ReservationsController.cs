using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<ReservationDomainModel>>> GetAsync()
        {
            IEnumerable<ReservationDomainModel> reservations = await _reservationService.GetAllAsync();

            if(reservations == null)
            {
                return null;
            }

            return Ok(reservations);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<ReservationDomainModel>>> GetAsync([FromBody] CreateReservationModel createReservation)
        {
            ReservationDomainModel reservationDomain = new ReservationDomainModel()
            {
                ProjectionId = createReservation.ProjectionId,
                ProjectionTime = createReservation.ProjectionTime,
                UserId = createReservation.UserId,
                Seats = createReservation.Seats
            };

            ReservationDomainModel reservations = await _reservationService.CreateReservation(reservationDomain);

            if (reservations == null)
            {
                return null;
            }

            return Ok(reservations);
        }
    }
}
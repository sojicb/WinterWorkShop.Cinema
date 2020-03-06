using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
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
        public async Task<ActionResult<IEnumerable<ReservationDomainModel>>> CreateReservation([FromBody] CreateReservationModel createReservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ReservationDomainModel reservationDomain = new ReservationDomainModel()
            {
                AuditoriumId = createReservation.AuditoriumId,
                ProjectionId = createReservation.ProjectionId,
                ProjectionTime = createReservation.ProjectionTime,
                UserId = createReservation.UserId,
                Seats = createReservation.Seats
            };

            var reservations = await _reservationService.HandleSeatValidation(reservationDomain);

           
             if(!reservations.IsSuccessful)
             {
                ErrorResponseModel errorResponseModel = new ErrorResponseModel
                {
                    ErrorMessage = reservations.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return BadRequest(errorResponseModel);
             }
            
            

            CreateReservationResultModel reservation;

            try
            {
                reservation = await _reservationService.CreateReservation(reservationDomain);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (!reservation.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = reservation.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Created("reservations//" + reservation.Reservation.Id, reservation.Reservation);
        }
    }
}
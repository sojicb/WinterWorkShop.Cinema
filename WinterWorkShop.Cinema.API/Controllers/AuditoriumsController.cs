using System;
using System.Collections.Generic;
using System.Data.Common;
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
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditoriumsController : ControllerBase
    {
        private readonly IAuditoriumService _auditoriumService;

        public AuditoriumsController(IAuditoriumService auditoriumservice)
        {
            _auditoriumService = auditoriumservice;
        }

        /// <summary>
        /// Gets all auditoriums
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<AuditoriumDomainModel>>> GetAsync()
        {
            IEnumerable<AuditoriumDomainModel> auditoriumDomainModels;

            auditoriumDomainModels = await _auditoriumService.GetAllAsync();

            if (auditoriumDomainModels == null)
            {
                auditoriumDomainModels = new List<AuditoriumDomainModel>();
            }

            return Ok(auditoriumDomainModels);
        }

        /// <summary>
        /// Gets Movie by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get/{id}")]
        public async Task<ActionResult<AuditoriumDomainModel>> GetAsyncById(int id)
        {
            AuditoriumDomainModel auditorium;

            auditorium = await _auditoriumService.GetAuditroiumByIdAsync(id);

            if (auditorium == null)
            {
                return NotFound(Messages.AUDITORIUM_DOES_NOT_EXIST);
            }

            return Ok(auditorium);
        }

        /// <summary>
        /// Adds a new auditorium
        /// </summary>
        /// <param name="createAuditoriumModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<AuditoriumDomainModel>> PostAsync(CreateAuditoriumModel createAuditoriumModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AuditoriumDomainModel auditoriumDomainModel = new AuditoriumDomainModel
            {
                CinemaId = createAuditoriumModel.cinemaId,
                Name = createAuditoriumModel.auditName,

            };

            CreateAuditoriumResultModel createAuditoriumResultModel;

            try
            {
                createAuditoriumResultModel = await _auditoriumService.CreateAuditorium(auditoriumDomainModel, createAuditoriumModel.numberOfSeats, createAuditoriumModel.seatRows);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (!createAuditoriumResultModel.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = createAuditoriumResultModel.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Created("auditoriums//" + createAuditoriumResultModel.Auditorium.Id, createAuditoriumResultModel);
        }

        /// <summary>
        /// Updates auditorium
        /// </summary>
        /// <param name="createAuditoriumModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "admin")]
        [Route("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody]CreateAuditoriumModel createAuditoriumModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AuditoriumDomainModel auditoriumToUpdate;

            auditoriumToUpdate = await _auditoriumService.GetAuditoriumByIdAsync(id);

            if (auditoriumToUpdate == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.AUDITORIUM_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);

            }

            auditoriumToUpdate.CinemaId = createAuditoriumModel.cinemaId;
            auditoriumToUpdate.Name = createAuditoriumModel.auditName;


            AuditoriumDomainModel auditoriumDomainModel;
            try
            {
                auditoriumDomainModel = await _auditoriumService.UpdateAuditorium(auditoriumToUpdate);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Accepted("auditroiums//" + auditoriumDomainModel.Id, auditoriumDomainModel);
        }

        /// <summary>
        /// Delets auditorium
        /// </summary>
        /// <param name="createAuditoriumModel"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "admin")]
        [Route("{id}")]
        public async Task<ActionResult> Delete(int id)
        {

            DeleteAuditoriumDomainModel deleteAuditorium;
            try
            {
                deleteAuditorium = await _auditoriumService.DeleteAuditorium(id);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }


            if (deleteAuditorium == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.AUDITORIUM_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("auditroiums//" + deleteAuditorium.Auditorium.Id, deleteAuditorium.Auditorium);

        }
    }
}
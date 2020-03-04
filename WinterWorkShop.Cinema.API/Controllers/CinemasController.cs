using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CinemasController : ControllerBase
    {
        private readonly ICinemaService _cinemaService;

        public CinemasController(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        /// <summary>
        /// Gets all cinemas
        /// </summary>
        /// <returns>List of cinemas</returns>
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<CinemaDomainModel>>> GetAsync()
        {
            IEnumerable<CinemaDomainModel> cinemaDomainModels;

            cinemaDomainModels = await _cinemaService.GetAllAsync();

            if (cinemaDomainModels == null)
            {
                cinemaDomainModels = new List<CinemaDomainModel>();
            }

            return Ok(cinemaDomainModels);
        }

        /// <summary>
        /// Gets Movie by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get/{id}")]
        public async Task<ActionResult<CinemaDomainModel>> GetAsyncById(int id)
        {
            CinemaDomainModel cinema;

            cinema = await _cinemaService.GetCinemaByIdAsync(id);

            if (cinema == null)
            {
                return NotFound(Messages.CINEMA_DOES_NOT_EXIST_ERROR);
            }

            return Ok(cinema);
        }

        /// <summary>
        /// Adds a new movie
        /// </summary>
        /// <param name="cinemaModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]CinemaModels cinemaModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CinemaDomainModel domainModel = new CinemaDomainModel
            {
                Name = cinemaModel.Name
            };

            CreateCinemaResultModel createCienema;

            try
            {
                createCienema = await _cinemaService.AddCinema(domainModel);
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

            if (createCienema == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_CREATION_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Created("cinemas//" + createCienema.Cinema.Id, createCienema.Cinema);
        }

        /// <summary>
        /// Adds a new movie
        /// </summary>
        /// <param name="cinemaModel"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "admin")]
        [Route("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            DeleteCinemaDomainModel deleteCinemaModel;

            try
            {
                deleteCinemaModel = await _cinemaService.DeleteCinema(id);
            }
            catch(DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (!deleteCinemaModel.IsSuccessful)
            {
                ErrorResponseModel error = new ErrorResponseModel
                {
                    ErrorMessage = Messages.PROJECTION_IN_FUTURE_ON_CINEMA_DELETE,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, error);
            }

            if(deleteCinemaModel == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.CINEMA_DOES_NOT_EXIST_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("cinemas//" + deleteCinemaModel.Cinema.Id, deleteCinemaModel.Cinema);
        }

        /// <summary>
        /// Updates cinemas
        /// </summary>
        /// <param name="createAuditoriumModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "admin")]
        [Route("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody]CinemaModels cinemaModels)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CinemaDomainModel cinemaToUpdate;

            cinemaToUpdate = await _cinemaService.GetCinemaByIdAsync(id);

            if (cinemaToUpdate == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.CINEMA_DOES_NOT_EXIST_ERROR,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);

            }

            cinemaToUpdate.Name = cinemaModels.Name;

            CinemaDomainModel cinemaDomainModel;
            try
            {
                cinemaDomainModel = await _cinemaService.UpdateCinema(cinemaToUpdate);
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

            return Accepted("cinemas//" + cinemaDomainModel.Id, cinemaDomainModel);
        }
    }
}

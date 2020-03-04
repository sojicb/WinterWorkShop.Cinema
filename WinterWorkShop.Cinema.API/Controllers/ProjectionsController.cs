using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectionsController : ControllerBase
    {
        private readonly IProjectionService _projectionService;

        public ProjectionsController(IProjectionService projectionService)
        {
            _projectionService = projectionService;
        }

        /// <summary>
        /// Gets all projections
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainModel>>> GetAsync()
        {
            IEnumerable<ProjectionDomainModel> projectionDomainModels;

            projectionDomainModels = await _projectionService.GetAllAsync();

            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<ProjectionDomainModel>();
            }

            return Ok(projectionDomainModels);
        }


        /// <summary>
        /// Gets Projection by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get/{id}")]
        public async Task<ActionResult<ProjectionDomainModel>> GetAsyncById(Guid id)
        {
            ProjectionDomainModel projection;

            projection = await _projectionService.GetProjectionByIdAsync(id);

            if (projection == null)
            {
                return NotFound(Messages.PROJECTION_DOES_NOT_EXIST);
            }

            return Ok(projection);
        }


        /// <summary>
        /// Adds a new projection
        /// </summary>
        /// <param name="projectionModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ProjectionDomainModel>> PostAsync(CreateProjectionModel projectionModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (projectionModel.ProjectionTime < DateTime.Now)
            {
                ModelState.AddModelError(nameof(projectionModel.ProjectionTime), Messages.PROJECTION_IN_PAST);
                return BadRequest(ModelState);
            }

            ProjectionDomainModel domainModel = new ProjectionDomainModel
            {
                AuditoriumId = projectionModel.AuditoriumId,
                MovieId = projectionModel.MovieId,
                ProjectionTime = projectionModel.ProjectionTime
            };

            CreateProjectionResultModel createProjectionResultModel;

            try
            {
                createProjectionResultModel = await _projectionService.CreateProjection(domainModel);
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

            if (!createProjectionResultModel.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = createProjectionResultModel.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Created("projections//" + createProjectionResultModel.Projection.Id, createProjectionResultModel.Projection);
        }


        /// <summary>
        /// Updates a projection
        /// </summary>
        /// <param name="projectionModel"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "admin")]
        [Route("update/{id}")]
        public async Task<ActionResult> Put(Guid id, [FromBody] UpdateProjectionModel projectionModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ProjectionDomainModel projectionToUpdate;

            projectionToUpdate = await _projectionService.GetProjectionByIdAsync(id);

            if (projectionToUpdate == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.PROJECTION_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            projectionToUpdate.AuditoriumId = projectionModel.AuditoriumId;
            projectionToUpdate.ProjectionTime = projectionModel.ProjectionTime;

            ProjectionDomainModel projectionDomainModel;
            try
            {
                projectionDomainModel = await _projectionService.UpdateProjection(projectionToUpdate);
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

            return Accepted("projections//" + projectionDomainModel.Id, projectionDomainModel);
        }

        /// <summary>
        /// Deletes a projection by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            ProjectionDomainModel deleteProjection;
            try
            {
                deleteProjection = _projectionService.DeleteProjection(id);
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

            if (deleteProjection == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.PROJECTION_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("projections//" + deleteProjection.Id, deleteProjection);
        }

        /// <summary>
        /// Filters projection
        /// </summary>
        /// <param name="auditoriumId"></param>
        /// <param name="cinemaId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("filtering")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainModel>>> FilterProjections(int ? cinemaId = null, int ? auditoriumId = null, Guid ? movieId = null, DateTime ? dateFrom = null, DateTime ? dateTo = null)
        {

            IEnumerable<ProjectionDomainModel> projectionDomainModels = await _projectionService.FilterProjections(cinemaId, auditoriumId, movieId, dateFrom, dateTo);

            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<ProjectionDomainModel>();
            }

            return Ok(projectionDomainModels);
        }

    }
}
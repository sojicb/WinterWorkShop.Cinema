using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Data;
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
        /// Gets Movie by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get/{id}")]
        public async Task<ActionResult<ProjectionDomainModel>> GetAsyncById(Guid id)
        {
            ProjectionDomainModel projection;

            projection = await _projectionService.GetProjectionById2Async(id);

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
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "admin")]
        [Route("{id}")]
        public async Task<ActionResult> Put(Guid id, [FromBody] CreateProjectionModel projectionModel)
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
            projectionToUpdate.MovieId = projectionModel.MovieId;
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


        [HttpDelete]
        [Route("{id}")]
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
        /// Gets all projections
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("filter")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainModel>>> FilterProjections([FromBody]FilterModel filter)
        {

            List<ProjectionDomainModel> projections = new List<ProjectionDomainModel>();
            if (filter.Projections != null)
            {
                foreach (var projection in filter.Projections)
                {
                    projections.Add(new ProjectionDomainModel
                    {
                        Id = projection.Id,
                        AditoriumName = projection.AditoriumName,
                        AuditoriumId = projection.AuditoriumId,
                        MovieId = projection.MovieId,
                        MovieTitle = projection.MovieTitle,
                        ProjectionTime = projection.ProjectionTime
                    });
                }
            }

            FilterDomainModel filterDomain = new FilterDomainModel
            {
                AuditoriumId = filter.AuditoriumId,
                CinemaId = filter.CinemaId,
                MovieId = filter.MovieId,
                ProjectionDateFrom = filter.ProjectionDateFrom,
                ProjectionDateTo = filter.ProjectionDateTo,
                Projections = projections
            };

            IEnumerable<ProjectionDomainModel> projectionDomainModels = await _projectionService.FilterProjections(filterDomain);

            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<ProjectionDomainModel>();
            }


            return Ok(projectionDomainModels);
        }

        /// <summary>
        /// Gets all projections
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("filtering")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainModel>>> FilterProjectionsTwo(int cinemaId, int auditoriumId, Guid ? movieId = null, DateTime ? dateFrom = null, DateTime ? dateTo = null)
        {

            IEnumerable<ProjectionDomainModel> projectionDomainModels = await _projectionService.FilterProjectionsTwo(cinemaId, auditoriumId, movieId, dateFrom, dateTo);

            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<ProjectionDomainModel>();
            }

            return Ok(projectionDomainModels);
        }

    }
}
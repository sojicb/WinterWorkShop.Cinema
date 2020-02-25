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
        /// Adds a new projection
        /// </summary>
        /// <param name="projectionModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("create")]
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


        [HttpPut]
        [Authorize(Roles = "admin")]
        [Route("update/{id}")]
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
        [Route("delete/{id}")]
        [Authorize(Roles ="admin")]
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

        


        [HttpGet]
        [Route("filteringProjections")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainModel>>> FilteringProjections([FromBody]object obj)
        {
            IEnumerable<ProjectionDomainModel> projCinema = await _projectionService.FilterProjectionsByCinemas();
            IEnumerable<ProjectionDomainModel> projAudit = await _projectionService.FilterProjectionsByAuditoriums();
            IEnumerable<ProjectionDomainModel> projMovie = await _projectionService.FilterProjectionsByMovies();
            IEnumerable<ProjectionDomainModel> projDateime = await _projectionService.FilterProjectionsBySpecificTime();

            if (projCinema == obj)
            {
                projCinema = new List<ProjectionDomainModel>();
                return Ok(projCinema);
            }
            else if (projAudit == obj)
            {
                projAudit = new List<ProjectionDomainModel>();
                return Ok(projAudit);
            }
            else if (projMovie == obj)
            {
                projMovie = new List<ProjectionDomainModel>();
                return Ok(projMovie);
            }
            else if (projDateime == obj)
            {
                projDateime = new List<ProjectionDomainModel>();
                return Ok(projDateime);
            }

            return BadRequest();
        }
    }
}
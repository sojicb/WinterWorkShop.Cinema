using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }


        /// <summary>
        /// Gets all projections
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<TagDomainModel>>> GetAsync()
        {
            IEnumerable<TagDomainModel> tagsDomainModels;

            tagsDomainModels = await _tagService.GetAllAsync();

            if (tagsDomainModels == null)
            {
                tagsDomainModels = new List<TagDomainModel>();
            }

            return Ok(tagsDomainModels);
        }


        /// <summary>
        /// Gets Movie by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get/{id}")]
        public async Task<ActionResult<TagDomainModel>> GetAsync(int id)
        {
            TagDomainModel movieTags;

            movieTags = await _tagService.GetTagByIdAsync(id);

            if (movieTags == null)
            {
                return NotFound(Messages.TAG_DOES_NOT_EXIST);
            }

            return Ok(movieTags);
        }


        /// <summary>
        /// Adds a new movie
        /// </summary>
        /// <param name="movieModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<CreateTagResultModel>> Post([FromBody]TagModel tagModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TagDomainModel domainModel = new TagDomainModel
            {
                Id = tagModel.Id,
                value = tagModel.Value
            };

            CreateTagResultModel createTag = new CreateTagResultModel();

            try
            {
                createTag = await _tagService.AddTag(domainModel);
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

            if (createTag == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.TAG_CREATION_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Created("movies//" + createTag.Tag.Id, createTag.Tag);
        }



        /// <summary>
        /// Updates a movie
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tagModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody]TagModel tagModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TagDomainModel tagToUpdate;

            tagToUpdate = await _tagService.GetTagByIdAsync(id);

            if (tagToUpdate == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.TAG_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            tagToUpdate.Id = tagModel.Id;
            tagToUpdate.value = tagModel.Value;
            

            TagDomainModel tagDomainModel;
            try
            {
                tagDomainModel = await _tagService.UpdateTag(tagToUpdate);
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

            return Accepted("tags//" + tagDomainModel.Id, tagDomainModel);

        }


        /// <summary>
        /// Delete a movie by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            TagDomainModel deletedTag;
            try
            {
                deletedTag = await _tagService.DeleteTag(id);
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

            if (deletedTag == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("tags//" + deletedTag.Id, deletedTag);
        }

    }
}

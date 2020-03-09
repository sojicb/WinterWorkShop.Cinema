using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IProjectionService _projectionService;

        private readonly ILogger<MoviesController> _logger;

        public MoviesController(ILogger<MoviesController> logger, IMovieService movieService, IProjectionService projectionService)
        {
            _logger = logger;
            _movieService = movieService;
            _projectionService = projectionService;
        }

        /// <summary>
        /// Gets Movie by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<MovieDomainModel>> GetAsync(Guid id)
        {
            MovieDomainModel movie = await _movieService.GetMovieByIdAsync(id);

            if (movie == null)
            {
                return NotFound(Messages.MOVIE_DOES_NOT_EXIST);
            }

            return Ok(movie);
        }

        /// <summary>
        /// Gets all Movies by Tag
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("tag/{id}")]
        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> GetMoviesByTag(int id)
        {
            var movies = await _movieService.GetMoviesByTag(id);

            if (movies == null)
            {
                return NotFound(Messages.MOVIE_INVALID_TAG);
            }

            return Ok(movies);
        }

        /// <summary>
        /// Gets all current movies
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("current")]
        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> GetMoviesAsync()
        {
            IEnumerable<MovieDomainModel> movieDomainModels;
            
            movieDomainModels = await _movieService.GetCurrentMovies(true);

            if (movieDomainModels == null)
            {
                movieDomainModels = new List<MovieDomainModel>();
            }

            return Ok(movieDomainModels);
        }

        /// <summary>
        /// Gets all movies
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> GetAllMovies()
        {
            IEnumerable<MovieDomainModel> movieDomainModels;

            movieDomainModels = await _movieService.GetAll();

            if (movieDomainModels == null)
            {
                movieDomainModels = new List<MovieDomainModel>();
            }

            return Ok(movieDomainModels);
        }

        /// <summary>
        /// Adds a new movie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("change/{id}")]
        public async Task<ActionResult> ChangeCurrent(Guid id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MovieDomainModel movieToUpdate;

            movieToUpdate = await _movieService.GetMovieByIdAsync(id);
            var projections = await _projectionService.GetAllAsync();

            var projection = projections.Where(x => x.MovieId.Equals(id));

            if (movieToUpdate == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (projection.Any(x => x.ProjectionTime > DateTime.Now))
            {

                return BadRequest(Messages.PROJECTION_IN_FUTURE);
            }

            if (movieToUpdate.Current == true)
            {
                movieToUpdate.Current = false;
            }
            else
            {
                movieToUpdate.Current = true;
            }
           

            MovieDomainModel movieDomainModel;
            try
            {
                movieDomainModel = await _movieService.UpdateMovie(movieToUpdate);
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

            return Accepted("movies//" + movieDomainModel.Id, movieDomainModel);
        }

        /// <summary>
        /// Adds a new movie
        /// </summary>
        /// <param name="movieModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<CreateMovieResultModel>> Post([FromBody]MovieModel movieModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MovieDomainModel domainModel = new MovieDomainModel
            {
                Current = movieModel.Current,
                Rating = movieModel.Rating,
                Title = movieModel.Title,
                Year = movieModel.Year
            };

            CreateMovieResultModel createMovie = new CreateMovieResultModel();

            try
            {
                createMovie = await _movieService.AddMovie(domainModel);
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

            if (createMovie == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_CREATION_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Created("movies//" + createMovie.Movie.Id, createMovie.Movie);
        }

        /// <summary>
        /// Updates a movie
        /// </summary>
        /// <param name="id"></param>
        /// <param name="movieModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Put(Guid id, [FromBody]MovieModel movieModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MovieDomainModel movieToUpdate;

            movieToUpdate = await _movieService.GetMovieByIdAsync(id);

            if (movieToUpdate == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            movieToUpdate.Title = movieModel.Title;
            movieToUpdate.Current = movieModel.Current;
            movieToUpdate.Year = movieModel.Year;
            movieToUpdate.Rating = movieModel.Rating;

            MovieDomainModel movieDomainModel;
            try
            {
                movieDomainModel = await _movieService.UpdateMovie(movieToUpdate);
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

            return Accepted("movies//" + movieDomainModel.Id, movieDomainModel);
        }

        /// <summary>
        /// Delete a movie by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            MovieDomainModel deletedMovie;
            try
            {
                deletedMovie = await _movieService.DeleteMovie(id);
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

            if (deletedMovie == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("movies//" + deletedMovie.Id, deletedMovie);
        }

        /// <summary>
        /// Gets top 10 movies
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Route("topmovies")]
        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> GetTopMovies()
        {
            IEnumerable<MovieDomainModel> movieDomainModels;

            movieDomainModels = await _movieService.MovieTopList();

            if (movieDomainModels == null)
            {
                movieDomainModels = new List<MovieDomainModel>();
            }

            return Ok(movieDomainModels);
        }

        /// <summary>
        /// Returns all movies by auditorium id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("filterMovies/{id}")]
        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> GetMoviesByAuditId(int id)
        {
            IEnumerable<MovieDomainModel> movieDomainModels = await _movieService.GetMoviesByAuditId(id);

            if(movieDomainModels == null)
            {
                movieDomainModels = new List<MovieDomainModel>();
            }

            return Ok(movieDomainModels);
        }

        /// <summary>
        /// Gets movies with projections in the future
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Route("futureProjections")]
        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> GetMoviesWithProjectionsInFuture()
        {
            IEnumerable<MovieDomainModel> movieDomainModels = await _movieService.GetMoviesWithProjectionsInFuture();

            if (movieDomainModels == null)
            {
                movieDomainModels = new List<MovieDomainModel>();
            }

            return Ok(movieDomainModels);
        }
    }
}

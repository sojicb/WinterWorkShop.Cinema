using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class MovieControllerTests
    {
        private Mock<IMovieService> _movieService;
        private Mock<IProjectionService> _projectionService;
       

        [TestMethod]
        public void GetAsync_Return_All_Movies()
        {

            List<TagDomainModel> tags = new List<TagDomainModel>();
            foreach (var item in tags)
            {
                tags.Add(new TagDomainModel
                {
                    Id = 1,
                    value = "Naziv taga"
                });
            }

            //Arrange
            List<MovieDomainModel> movieDomainModelList = new List<MovieDomainModel>();
            MovieDomainModel movieDomainModel = new MovieDomainModel
            {
                Id = Guid.NewGuid(),
                Title = "Movie title",
                Current = true,
                Rating = 7,
                Year = 2000,
                Tags = tags
            };

            movieDomainModelList.Add(movieDomainModel);
            IEnumerable<MovieDomainModel> movieDomainModels = movieDomainModelList;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _movieService = new Mock<IMovieService>();
            _projectionService = new Mock<IProjectionService>();

            _movieService.Setup(x => x.GetAll()).Returns(responseTask);
            MoviesController moviesController = new MoviesController( _movieService.Object, _projectionService.Object);

            //Act
            var result = moviesController.GetAllMovies().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.AreEqual(movieDomainModel.Id, movieDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_NewList()
        {
            //Arrange 
            IEnumerable<MovieDomainModel> movieDomainModels = null;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _movieService = new Mock<IMovieService>();
            _projectionService = new Mock<IProjectionService>();
            _movieService.Setup(x => x.GetAll()).Returns(responseTask);
            MoviesController moviesController = new MoviesController( _movieService.Object, _projectionService.Object);

            //Act
            var result = moviesController.GetAllMovies().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var projectionDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert    
            Assert.IsNotNull(projectionDomainModelResultList);
            Assert.AreEqual(expectedResultCount, projectionDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }



        [TestMethod]
        public void PostAsync_Create_Throw_DbException_Projection()
        {
            List<TagDomainModel> tags = new List<TagDomainModel>();
            foreach (var item in tags)
            {
                tags.Add(new TagDomainModel
                {
                    Id = 1,
                    value = "Naziv taga"
                });
            }

            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;

            MovieModel movieModel = new MovieModel()
            {
                Title = "Movie title",
                Rating = 8,
                Current = true,
                Year = 1997
            };
            CreateMovieResultModel createMovieResultModel = new CreateMovieResultModel
            {
                Movie = new MovieDomainModel
                {
                    Id = Guid.NewGuid(),
                    Current = true,
                    Rating = 8,
                    Title = "Movie title",
                    Year = 1997,
                    Tags = tags

                },
                IsSuccessful = true
            };
            Task<CreateMovieResultModel> responseTask = Task.FromResult(createMovieResultModel);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _movieService = new Mock<IMovieService>();
            _projectionService = new Mock<IProjectionService>();
            _movieService.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>())).Throws(dbUpdateException);
            MoviesController moviesController = new MoviesController( _movieService.Object, _projectionService.Object);

            //Act
            var result = moviesController.Post(movieModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void PostAsync_Create_createMovieResultModel_IsSuccessful_True_Movies()
        {
            //Arrange
            int expectedStatusCode = 201;

            MovieModel movieModel = new MovieModel()
            {
                Title = "Movie title",
                Rating = 8,
                Year = 2010,
                Current = true
            };

            CreateMovieResultModel createMovieResultModel = new CreateMovieResultModel()
            {
                Movie = new MovieDomainModel
                {
                   Id = Guid.NewGuid(),
                   Title = movieModel.Title,
                   Rating = movieModel.Rating,
                   Year = movieModel.Year,
                   Current = movieModel.Current
                   
                },
                IsSuccessful = true
            };

            Task<CreateMovieResultModel> responseTask = Task.FromResult(createMovieResultModel);

            _movieService = new Mock<IMovieService>();
            _projectionService = new Mock<IProjectionService>();
            _movieService.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_movieService.Object, _projectionService.Object);

            //Act
            var result = moviesController.Post(movieModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var createResult = ((CreatedResult)result).Value;
            var movieDomainModel = (MovieDomainModel)createResult;

            //Assert
            Assert.IsNotNull(movieDomainModel);
            Assert.AreEqual(movieModel.Title, movieDomainModel.Title);
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)result).StatusCode);
        }

    }
}

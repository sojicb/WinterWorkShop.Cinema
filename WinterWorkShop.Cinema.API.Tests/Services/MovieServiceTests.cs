using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
	[TestClass]
	public class MovieServiceTests
	{
		private Mock<IMoviesRepository> _mockMoviesRepository;
		private Movie _movie;
		private MovieDomainModel _movieDomainModel;
		private MovieDomainModel _updateMovie;

		[TestInitialize]
		public void TestInitialize()
		{
			_movie = new Movie
			{
				Id = Guid.NewGuid(),
				Title = "ProbaFilm",
				Rating = 10,
				Year = 2000,
				Current = true
			};

			_movieDomainModel = new MovieDomainModel
			{
				Id = Guid.NewGuid(),
				Current = true,
				Rating = 10,
				Year = 2099,
				Title = "ProbaDomainFilm"
			};

			_updateMovie = new MovieDomainModel
			{
				Id = Guid.NewGuid(),
				Current = true,
				Rating = 8,
				Year = 2066,
				Title = "UpdateFilm"
			};

			List<Movie> movieModelsList = new List<Movie>();

			movieModelsList.Add(_movie);
			IEnumerable<Movie> movies = movieModelsList;
			Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);

			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_mockMoviesRepository.Setup(x => x.GetCurrentMovies()).Returns(responseTask.Result);
		}

		[TestMethod]
		public void MoviesService_GetAllCurrentMovies_ReturnsListOfMovies()
		{
			//Arrange
			int expectedResults = 1;
			MovieService movieController = new MovieService(_mockMoviesRepository.Object);

			//Act
			var resultAction = movieController.GetAllMovies(false).ConfigureAwait(false).GetAwaiter().GetResult();
			var result = (List<MovieDomainModel>)resultAction;

			//Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(expectedResults, result.Count);
			Assert.AreEqual(_movie.Id, result[0].Id);
		}

		[TestMethod]
		public void MoviesService_GetAllCurrentMovies_ReturnsNull()
		{
			//Arrange
			IEnumerable<Movie> movies = null;
			Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);

			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_mockMoviesRepository.Setup(x => x.GetCurrentMovies()).Returns(responseTask.Result);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object);

			//Act
			var resultAction = movieService.GetAllMovies(true).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsNull(resultAction);
		}

		[TestMethod]
		public void MoviesServices_GetMovieById_ReturnsMovie()
		{
			//arrange
			Movie movie = _movie;
			Task<Movie> responseTask = Task.FromResult(movie);

			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_mockMoviesRepository.Setup(x => x.GetByIdAsync(movie.Id)).Returns(responseTask);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object);

			//act
			var resultAction = movieService.GetMovieByIdAsync(_movie.Id).ConfigureAwait(false).GetAwaiter().GetResult();
			var result = (MovieDomainModel)resultAction;

			//assert
			Assert.AreEqual(_movie.Id, result.Id);
		}

		[TestMethod]
		public void MoviesService_GetMovieById_InsertedMockedNull_ReturnsNull()
		{
			//Arrange
			Movie movie = null;
			Task<Movie> responseTask = Task.FromResult(movie);

			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_mockMoviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTask);
			_mockMoviesRepository.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(_movie);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object);

			//Act
			var resultAction = movieService.GetMovieByIdAsync(_movie.Id).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsNull(resultAction);
		}

		[TestMethod]
		public void MovieService_CreateMovie_InsertMocked_ReturnMovie()
		{
			//Arrange
			_mockMoviesRepository = new Mock<IMoviesRepository>();

			_mockMoviesRepository.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(_movie);
			_mockMoviesRepository.Setup(x => x.Save());
			MovieService movieService = new MovieService(_mockMoviesRepository.Object);

			//Act
			var resultAction = movieService.AddMovie(_movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsNotNull(resultAction);
			Assert.AreEqual(_movie.Id, resultAction.Movie.Id);
			Assert.IsNull(resultAction.ErrorMessage);
			Assert.IsTrue(resultAction.IsSuccessful);
		}

		[TestMethod]
		public void MovieService_CreateMovie_InsertMockedNull_ReturnErrorMessage()
		{
			//Arrange
			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_movie = null;
			string expectedMessage = "Error occured while creating new movie, please try again.";

			//_mockMoviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(_movie);
			_mockMoviesRepository.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(_movie);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object);

			//Act
			var resultAction = movieService.AddMovie(_movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsNull(resultAction.Movie);
			Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
			Assert.IsFalse(resultAction.IsSuccessful);
		}

		[TestMethod]
		public void MovieService_UpdateMovie_GetUpdatedMovie()
		{
			//Arrange
			_mockMoviesRepository = new Mock<IMoviesRepository>();

			//_mockMoviesRepository.Setup(x => x.Insert(_movie)).Returns(_movie);
			//_mockMoviesRepository.Setup(x => x.Save());
			MovieService movieService = new MovieService(_mockMoviesRepository.Object);

			//Act
			var resultAction = movieService.UpdateMovie(_updateMovie).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsNotNull(resultAction);
			Assert.AreEqual(_updateMovie.Id, resultAction.Id);
		}

	}
}

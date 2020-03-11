using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
	[TestClass]
	public class MovieServiceTests
	{
		private Mock<IMoviesRepository> _mockMoviesRepository;
		private Mock<IMovieTagsService> _movieTagsService;
		private Mock<IProjectionsRepository> _projectionsRepository;
		private Movie _movie;
		private Movie _movieTwo;
		private Movie _movieThree;
		private Movie _movieFour;
		private MovieDomainModel _movieDomainModel;
		private MovieDomainModel _movieToDelete;
		private MovieDomainModel _updateMovie;

		[TestInitialize]
		public void TestInitialize()
		{
			_movie = new Movie
			{
				Id = Guid.NewGuid(),
				Title = "Proba",
				Rating = 10,
				Year = 2000,
				Current = true
			};

			_movieTwo = new Movie
			{
				Id = Guid.NewGuid(),
				Title = "Two",
				Rating = 8,
				Year = 2000,
				Current = true
			};

			_movieThree = new Movie
			{
				Id = Guid.NewGuid(),
				Title = "Three",
				Rating = 7,
				Year = 2000,
				Current = true
			};

			_movieFour = new Movie
			{
				Id = Guid.NewGuid(),
				Title = "Four",
				Rating = 9,
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
				Title = "Proba"
			};

			
		}

		[TestMethod]
		public void MoviesService_GetAllCurrentMovies_ReturnsListOfMovies()
		{
			//Arrange
			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();

			List<Movie> movieModelsList = new List<Movie>();

			movieModelsList.Add(_movie);
			IEnumerable<Movie> movies = movieModelsList;
			Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);

			_mockMoviesRepository.Setup(x => x.GetCurrentMovies()).Returns(responseTask.Result);
			int expectedResults = 1;
			MovieService movieController = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

			//Act
			var resultAction = movieController.GetCurrentMovies(true).ConfigureAwait(false).GetAwaiter().GetResult();
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
			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();
			IEnumerable<Movie> movies = null;
			Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);

			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_mockMoviesRepository.Setup(x => x.GetCurrentMovies()).Returns(responseTask.Result);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

			//Act
			var resultAction = movieService.GetCurrentMovies(true).ConfigureAwait(false).GetAwaiter().GetResult();

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
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();
			_mockMoviesRepository.Setup(x => x.GetByIdAsync(movie.Id)).Returns(responseTask);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

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
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();
			_mockMoviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTask);
			_mockMoviesRepository.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(_movie);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

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
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();

			_mockMoviesRepository.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(_movie);
			_mockMoviesRepository.Setup(x => x.Save());
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

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
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();
			_movie = null;
			string expectedMessage = "Error occured while creating new movie, please try again.";

			//_mockMoviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(_movie);
			_mockMoviesRepository.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(_movie);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

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
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();

			_mockMoviesRepository.Setup(x => x.Update(It.IsAny<Movie>())).Returns(_movie);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

			//Act
			var resultAction = movieService.UpdateMovie(_updateMovie).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsNotNull(resultAction);
			Assert.AreEqual(_updateMovie.Title, resultAction.Title);
		}

		[TestMethod]
		public void MovieService_UpdateMovie_InsertedNull_ReturnsError()
		{
			//Arrange
			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();
			_movie = null;

			_mockMoviesRepository.Setup(x => x.Update(It.IsAny<Movie>())).Returns(_movie);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

			//Act
			var resultAction = movieService.UpdateMovie(_updateMovie).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsNull(resultAction);
		}

		[TestMethod]
		public void MovieService_DeleteMovie_ReturnDeletedMovie()
		{
			//Arrange
			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();

			_mockMoviesRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(_movie);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

			//Act
			var resultAction = movieService.DeleteMovie(_movie.Id).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsNotNull(resultAction);
		}

		[TestMethod]
		public void MovieService_DeleteMovie_InsertedMockedNull_ReturnsNull()
		{
			//Arrange
			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();
			_movie = null;
			Guid movieId = new Guid();

			_mockMoviesRepository.Setup(x => x.Delete(It.IsAny<Movie>())).Returns(_movie);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

			//Act
			var resultAction = movieService.DeleteMovie(movieId).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsNull(resultAction);
		}

		[TestMethod]
		public void MovieService_MovieTopList_ReturnListOfTopMovies()
		{
			//Arrange
			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();
			List<Movie> movies = new List<Movie>();
			int expectedResult = 4;
			movies.Add(_movie);
			movies.Add(_movieTwo);
			movies.Add(_movieThree);
			movies.Add(_movieFour);
			IEnumerable<Movie> moviesList = movies;
			Task<IEnumerable<Movie>> responseTask = Task.FromResult(moviesList);

			_mockMoviesRepository.Setup(x => x.GetTopMovies()).Returns(responseTask);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

			//Act
			var resultAction = movieService.MovieTopList()
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult().ToList();

			//Assert
			Assert.IsNotNull(resultAction);
			Assert.AreEqual(resultAction.Count, expectedResult);
			Assert.IsTrue(resultAction[0].Rating > resultAction[1].Rating);
		}

		[TestMethod]
		public void MovieService_MovieTopList_InsertedMockeNull_ReturnsNull()
		{
			//Arrange
			IEnumerable<Movie> movies = null;
			Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);

			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();
			_mockMoviesRepository.Setup(x => x.GetTopMovies()).Returns(responseTask);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

			//Act
			var resultAction = movieService.MovieTopList().ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsNull(resultAction);
		}

		[TestMethod]
		public void MovieService_GetMoviesByTag_ReturnsMoviesByTag()
		{
			//Arrange
			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();
			var movies = new List<Movie>();
			movies.Add(_movie);
			movies.Add(_movieTwo);
			int id = 1;
			int expectedResult = 2;
			IEnumerable<Movie> MoviesWithTags = movies;
			Task<IEnumerable<Movie>> responseTask = Task.FromResult(MoviesWithTags);

			_mockMoviesRepository.Setup(x => x.GetMoviesByTag(id)).Returns(responseTask);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

			//Act
			var resultAction = movieService.GetMoviesByTag(id).ConfigureAwait(false).GetAwaiter().GetResult().ToList();

			//Assert
			Assert.IsNotNull(resultAction);
			Assert.AreEqual(expectedResult, resultAction.Count);
		}

		[TestMethod]
		public void MovieService_GetMoviesByTag_InsertedNonExistentTag_ReturnsErrorMessage()
		{
			//Arrange
			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();
			int id = 20;
			IEnumerable<Movie> movies = null;
			Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);

			_mockMoviesRepository.Setup(x => x.GetMoviesByTag(id)).Returns(responseTask);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

			//Act
			var resultAction = movieService.GetMoviesByTag(id)
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();

			//Assert
			Assert.IsNull(resultAction);
		}

		[TestMethod]
		public void MovieService_GetMoviesWithProjectionsInFuture_ReturnMoviesWithProjectionsInFuture()
		{
			//Arrange
			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();
			
			List<Projection> projections = new List<Projection>();
			projections.Add(new Projection
			{
				Id = new Guid(),
				MovieId = new Guid("09796BF4-D4B3-D0F5-0B69-006A5A9AD16B"),
				DateTime = DateTime.Parse("2023 - 10 - 20 00:07:57.590"),
				Movie = _movie
			});
			
			List<Movie> movies = new List<Movie>();
			movies.Add(new Movie
			{
				Current = true,
				Id = new Guid("09796BF4-D4B3-D0F5-0B69-006A5A9AD16B"),
				Rating = 10,
				Title = "Proba",
				Year = 2022,
				Projections = projections
			});

			IEnumerable<Movie> MoviesInFuture = movies;
			Task<IEnumerable<Movie>> responseTask = Task.FromResult(MoviesInFuture);

			_mockMoviesRepository.Setup(x => x.GetMoviesWithProjectionsInFuture()).Returns(responseTask);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

			//Act
			var resultAction = movieService.GetMoviesWithProjectionsInFuture()
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult()
				.ToList();

			//Assert
			Assert.IsNotNull(resultAction);
		}

		[TestMethod]
		public void MovieService_GetMoviesWithProjectionsInFuture_ReturnsNull()
		{
			//Arrange
			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();
			
			IEnumerable<Movie> MoviesInFuture = null;
			Task<IEnumerable<Movie>> responseTask = Task.FromResult(MoviesInFuture);

			_mockMoviesRepository.Setup(x => x.GetMoviesWithProjectionsInFuture()).Returns(responseTask);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

			//Act
			var resultAction = movieService.GetMoviesWithProjectionsInFuture().ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsNull(resultAction);
		}

		[TestMethod]
		public void MoviesService_GetAll_ReturnsMovies()
		{
			//Arrange
			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();
			List<Movie> allMovies = new List<Movie>();
			int expectedResult = 4;
			allMovies.Add(_movie);
			allMovies.Add(_movieTwo);
			allMovies.Add(_movieThree);
			allMovies.Add(_movieFour);
			IEnumerable<Movie> movies = allMovies;
			Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);

			_mockMoviesRepository.Setup(x => x.GetAll()).Returns(responseTask);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

			//Act
			var resultAction = movieService.GetAll().ConfigureAwait(false).GetAwaiter().GetResult().ToList();

			//Assert
			Assert.IsNotNull(resultAction);
			Assert.AreEqual(resultAction.Count, expectedResult);
		}

		[TestMethod]
		public void MoviesService_GetAll_ReturnsNull()
		{
			//Arrange
			_mockMoviesRepository = new Mock<IMoviesRepository>();
			_movieTagsService = new Mock<IMovieTagsService>();
			_projectionsRepository = new Mock<IProjectionsRepository>();
			IEnumerable<Movie> movies = null;
			Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);

			_mockMoviesRepository.Setup(x => x.GetCurrentMovies()).Returns(responseTask.Result);
			MovieService movieService = new MovieService(_mockMoviesRepository.Object, _movieTagsService.Object, _projectionsRepository.Object);

			//Act
			var resultAction = movieService.GetCurrentMovies(true).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			Assert.IsNull(resultAction);
		}
	}
}

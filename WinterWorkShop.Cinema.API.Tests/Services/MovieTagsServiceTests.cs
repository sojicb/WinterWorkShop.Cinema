using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
	[TestClass]
	public class MovieTagsServiceTests
	{
		private Mock<IMovieTagRepository> _mockMovieTagRepository = new Mock<IMovieTagRepository>();

        private MovieTags _movieTag;
        private Movie _movie;
        private Tag _tag;

        [TestInitialize]
        public void TestInitialize()
        {
            _movie = new Movie
            {
                Id = new Guid("F0091C18-C0FC-3D04-3972-9380285E190D"),
                Current = true,
                Rating = 10,
                Title = "Proba",
                Year = 2020
            };

            _tag = new Tag
            {
                Id = 1,
                Value = "ProbaOscar"
            };

            _movieTag = new MovieTags
            {
                Movie = _movie,
                MovieId = _movie.Id,
                Tag = _tag,
                TagId = _tag.Id
            };
        }

        [TestMethod]
        public void MovieTagsService_GetAllAsync_ReturnsMovieTags()
        {
            //Arrange
            int expectedResult = 1;
            List<MovieTags> mockedMovieTags = new List<MovieTags>();
            mockedMovieTags.Add(_movieTag);
            IEnumerable<MovieTags> movieTags = mockedMovieTags;
            Task<IEnumerable<MovieTags>> responseTask = Task.FromResult(movieTags);

            _mockMovieTagRepository.Setup(x => x.GetAll()).Returns(responseTask);

            MovieTagsService auditoriumController = new MovieTagsService(_mockMovieTagRepository.Object);

            //Act
            var resultAction = auditoriumController.GetAllAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()
                .ToList();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resultAction.Count, expectedResult);
        }

        [TestMethod]
        public void MovieTagsService_GetAllAsync_InsertedMockedNull_ReturnsNull()
        {
            //Arrange
            IEnumerable<MovieTags> movieTags = null;
            Task<IEnumerable<MovieTags>> responseTask = Task.FromResult(movieTags);

            _mockMovieTagRepository.Setup(x => x.GetAll()).Returns(responseTask);

            MovieTagsService auditoriumController = new MovieTagsService(_mockMovieTagRepository.Object);

            //Act
            var resultAction = auditoriumController.GetAllAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }
    }
}

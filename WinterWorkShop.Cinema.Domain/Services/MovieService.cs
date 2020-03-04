using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMoviesRepository _moviesRepository;
        private readonly IMovieTagsService _movieTagsService;
        private readonly IProjectionsRepository _projectionsRepository;

        public MovieService(IMoviesRepository moviesRepository, IMovieTagsService movieTagsService, IProjectionsRepository projectionsRepository)
        {
            _moviesRepository = moviesRepository;
            _movieTagsService = movieTagsService;
            _projectionsRepository = projectionsRepository;
        }

        public async Task<IEnumerable<MovieDomainModel>> GetAll()
        {
            var data = await _moviesRepository.GetAll();
            if (data == null)
            {
                return null;
            }

            List<MovieDomainModel> result = new List<MovieDomainModel>();
            MovieDomainModel model;
            foreach (var item in data)
            {
                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Id,
                    Rating = item.Rating ?? 0,
                    Title = item.Title,
                    Year = item.Year
                };
                result.Add(model);
            }

            return result;
        }

        public IEnumerable<MovieDomainModel> GetCurrentMovies(bool? isCurrent)
        {
            var data = _moviesRepository.GetCurrentMovies();

            if (data == null)
            {
                return null;
            }

            List<MovieDomainModel> result = new List<MovieDomainModel>();
            List<TagDomainModel> tagResults = new List<TagDomainModel>();
            MovieDomainModel model;
            foreach (var item in data)
            {
                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Id,
                    Rating = item.Rating ?? 0,
                    Title = item.Title,
                    Year = item.Year,
                    Tags = tagResults
                };
                result.Add(model);
            }

            return result;

        }

        public async Task<MovieDomainModel> GetMovieByIdAsync(Guid id)
        {
            var data = await _moviesRepository.GetByIdAsync(id);
            var tags = await _movieTagsService.GetAllAsync();

            if (data == null)
            {
                return null;
            }

            List<TagDomainModel> tagResults = new List<TagDomainModel>();

            foreach (var tag in tags)
            {
                if(tag.Movie.Id.Equals(data.Id))
                {
                    tagResults.Add(new TagDomainModel
                    {
                        Id = tag.TagId,
                        value = tag.Tag.value
                    });
                }
                
            }

            MovieDomainModel domainModel = new MovieDomainModel
            {
                Id = data.Id,
                Current = data.Current,
                Rating = data.Rating ?? 0,
                Title = data.Title,
                Year = data.Year,
                Tags = tagResults
            };

            return domainModel;
        }

        public async Task<CreateMovieResultModel> AddMovie(MovieDomainModel newMovie)
        {
            Movie movieToCreate = new Movie()
            {
                Title = newMovie.Title,
                Current = newMovie.Current,
                Year = newMovie.Year,
                Rating = newMovie.Rating
            };

            var createdMovie = _moviesRepository.Insert(movieToCreate);

            if (createdMovie == null)
            {
                return new CreateMovieResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.MOVIE_CREATION_ERROR
                };
            }

            _moviesRepository.Save();

            CreateMovieResultModel resultModel = new CreateMovieResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Movie = new MovieDomainModel
                {
                    Id = createdMovie.Id,
                    Current = createdMovie.Current,
                    Rating = createdMovie.Rating?? 0,
                    Title = createdMovie.Title,
                    Year = createdMovie.Year
                }
            };

            return resultModel;
        }

        public async Task<MovieDomainModel> UpdateMovie(MovieDomainModel updateMovie)
        {

            Movie movie = new Movie()
            {
                Id = updateMovie.Id,
                Title = updateMovie.Title,
                Current = updateMovie.Current,
                Year = updateMovie.Year,
                Rating = updateMovie.Rating
            };

            var data = _moviesRepository.Update(movie);

            if (data == null)
            {
                return null;
            }
           
            _moviesRepository.Save();

            MovieDomainModel domainModel = new MovieDomainModel()
            {
                Id = data.Id,
                Title = data.Title,
                Current = data.Current,
                Year = data.Year,
                Rating = data.Rating ?? 0
            };

            return domainModel;
        }

        public async Task<MovieDomainModel> DeleteMovie(Guid id)
        {
            var data = _moviesRepository.Delete(id);

            if (data == null)
            {
                return null;
            }

            _moviesRepository.Save();

            MovieDomainModel domainModel = new MovieDomainModel
            {
                Id = data.Id,
                Title = data.Title,
                Current = data.Current,
                Year = data.Year,
                Rating = data.Rating ?? 0

            };

            return domainModel;
        }

        public async Task<IEnumerable<MovieDomainModel>> MovieTopList()
        {
            var data = _moviesRepository.GetTopMovies();

            if (data == null)
            {
                return null;
            }

            List<MovieDomainModel> result = new List<MovieDomainModel>();
            MovieDomainModel model;
                foreach (var item in data)
                {
                if (result.Count == 10)
                {
                    break;
                }
                    model = new MovieDomainModel
                    {
                        Current = item.Current,
                        Id = item.Id,
                        Rating = item.Rating ?? 0,
                        Title = item.Title,
                        Year = item.Year
                    };
                    result.Add(model);
                }

            return result;
        }
        
        public IEnumerable<MovieDomainModel> GetMoviesByTag(int id)
        {
            var data = _moviesRepository.GetMoviesByTag(id).ToList();

            List<MovieDomainModel> movies = new List<MovieDomainModel>();

            if (data.Count == 0)
            {
                return null;
            }

            foreach(var movie in data)
            {
                movies.Add(new MovieDomainModel
                {
                    Current = movie.Current,
                    Id = movie.Id,
                    Rating = movie.Rating ?? 0,
                    Title = movie.Title,
                    Year = movie.Year,
                });
            }

            return movies;
        }

        public async Task<IEnumerable<MovieDomainModel>> GetMoviesByAuditId(int id)
        {
            var data = await _projectionsRepository.GetAll();

            if(data == null)
            {
                return null;
            }

            var projections = data.Select(x => x.Movie.Projections.Where(y => y.AuditoriumId.Equals(id))).ToList();
            var movieIds = projections.SelectMany(x => x.Select(y => y.MovieId));
            var movies = await _moviesRepository.GetAll();
            movies = movies.Where(x => movieIds.Contains(x.Id)).ToList();
            
            List<MovieDomainModel> models = new List<MovieDomainModel>();

            foreach(var movie in movies)
            {
                models.Add(new MovieDomainModel
                {
                    Current = movie.Current,
                    Id = movie.Id,
                    Rating = movie.Rating ?? 0,
                    Title = movie.Title,
                    Year = movie.Year
                });
            }

            return models;
        }

        public async Task<IEnumerable<MovieDomainModel>> GetMoviesWithProjectionsInFuture()
        {
            var result = await _moviesRepository.GetMoviesWithProjectionsInFuture();

            if(result == null)
            {
                return null;
            }

            List<MovieDomainModel> movies = new List<MovieDomainModel>();

            foreach(var movie in result)
            {
                movies.Add(new MovieDomainModel
                {
                    Current = movie.Current,
                    Id = movie.Id,
                    Rating = movie.Rating ?? 0,
                    Title = movie.Title,
                    Year = movie.Year,
                });
            }

            return movies;
        }
    }   
}   

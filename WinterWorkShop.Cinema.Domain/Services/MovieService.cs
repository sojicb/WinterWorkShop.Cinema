using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
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

        public MovieService(IMoviesRepository moviesRepository)
        {
            _moviesRepository = moviesRepository;
        }

        public async Task<IEnumerable<MovieDomainModel>> GetAllMovies(bool? isCurrent)
        {
            var data = _moviesRepository.GetCurrentMovies();

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

        public async Task<MovieDomainModel> GetMovieByIdAsync(Guid id)
        {
            var data = await _moviesRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            MovieDomainModel domainModel = new MovieDomainModel
            {
                Id = data.Id,
                Current = data.Current,
                Rating = data.Rating ?? 0,
                Title = data.Title,
                Year = data.Year
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
            var data = await _moviesRepository.GetTopMovies();

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
        
        public IEnumerable<CreateMovieResultModel> GetMoviesByTag(string tagValue)
        {
            var data = _moviesRepository.GetMoviesByTag(tagValue).ToList();

            List<CreateMovieResultModel> movies = new List<CreateMovieResultModel>();

            if (data.Count == 0)
            {
                movies.Add(new CreateMovieResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.MOVIE_INVALID_TAG
                });
                return movies;
            }

            foreach(var movie in data)
            {
                
                movies.Add(new CreateMovieResultModel
                {
                    IsSuccessful = true,
                    ErrorMessage = null,
                    Movie = new MovieDomainModel
                    {
                        Current = movie.Current,
                        Id = movie.Id,
                        Rating = movie.Rating ?? 0,
                        Title = movie.Title,
                        Year = movie.Year,
                        
                    }
                });
            }

            return movies;
        }
    }   
}   

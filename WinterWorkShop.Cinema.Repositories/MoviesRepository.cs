using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IMoviesRepository : IRepository<Movie> 
    {
        IEnumerable<Movie> GetCurrentMovies();
        IEnumerable<Movie> GetTopMovies();
        IEnumerable<Movie> GetMoviesByTag(int id);
        Task<IEnumerable<Movie>> GetMoviesWithProjectionsInFuture();
    }

    public class MoviesRepository : IMoviesRepository
    {
        private CinemaContext _cinemaContext;

        public MoviesRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public Movie Delete(object id)
        {
            Movie existing = _cinemaContext.Movies.Find(id);

            if (existing == null)
            {
                return null;
            }

            var result = _cinemaContext.Movies.Remove(existing);

            return result.Entity;
        }

        public async Task<IEnumerable<Movie>> GetAll()
        {
            return await _cinemaContext.Movies.Include(x => x.Projections).ToListAsync();
        }

        public async Task<Movie> GetByIdAsync(object id)
        {
            var data = await _cinemaContext.Movies.FindAsync(id);

            return data;
        }

        public IEnumerable<Movie> GetCurrentMovies()
        {
            var data = _cinemaContext.Movies.Where(x => x.Current).Include(x => x.Projections);

            return data;
        }

        public IEnumerable<Movie> GetMoviesByTag(int id)
        {
            var data = _cinemaContext.Movies.Where(x => x.MovieTags != null && x.MovieTags.Any(t => t.Tag.Id.Equals(id))).ToList();
            
            return data;
        }

        public Movie Insert(Movie obj)
        {
            var data = _cinemaContext.Movies.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Movie Update(Movie obj)
        {
            var updatedEntry = _cinemaContext.Movies.Attach(obj).Entity;
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry;
        }

        public IEnumerable<Movie> GetTopMovies()
        {
            var result = _cinemaContext.Movies.OrderByDescending(x => x.Rating);

            return result;
        }

        public async Task<IEnumerable<Movie>> GetMoviesWithProjectionsInFuture()
        {
            var movies = await _cinemaContext.Movies
                .Include(x => x.Projections)
                .Where(x => x.Projections.Any(y => y.DateTime > DateTime.Now))
                .ToListAsync();

           // var result = movies.Where(x => x.Projections.Where(y => y.DateTime > DateTime.Now)).ToList();

            return movies;
        }
    }
}

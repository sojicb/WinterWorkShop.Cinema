using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Repositories
{
	public interface IMovieTagRepository : IRepository<MovieTags>
	{

	}
	public class MovieTagsRepository : IMovieTagRepository
	{

		private CinemaContext _cinemaContext;

		public MovieTagsRepository(CinemaContext cinemaContext)
		{
			_cinemaContext = cinemaContext;
		}

		public MovieTags Delete(object id)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<MovieTags>> GetAll()
		{
			var data = await _cinemaContext.MovieTags.Include(x => x.Movie).Include(x => x.Tag).ToListAsync();

			return data;
		}

		public Task<MovieTags> GetByIdAsync(object id)
		{
			throw new NotImplementedException();
		}

		public MovieTags Insert(MovieTags obj)
		{
			throw new NotImplementedException();
		}

		public void Save()
		{
			throw new NotImplementedException();
		}

		public MovieTags Update(MovieTags obj)
		{
			throw new NotImplementedException();
		}
	}
}

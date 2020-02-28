using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
	public class MovieTagsService : IMovieTagsService
	{
        private readonly IMovieTagRepository _movieTagRepository;

        public MovieTagsService(IMovieTagRepository movieTagRepository)
        {
			_movieTagRepository = movieTagRepository;
        }
        public async Task<IEnumerable<MovieTagDomainModel>> GetAllAsync()
		{
			var data = await _movieTagRepository.GetAll();

			if(data == null)
			{
				return null;
			}

			List<MovieTagDomainModel> movieTags = new List<MovieTagDomainModel>();

			foreach(var tag in data)
			{
				movieTags.Add(new MovieTagDomainModel
				{
					MovieId = tag.Movie.Id,
					TagId = tag.Tag.Id
				});
			}

			return movieTags;
		}
	}
}

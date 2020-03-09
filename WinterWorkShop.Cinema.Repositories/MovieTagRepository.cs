using System;
using System.Collections.Generic;
using System.Text;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IMovieTagsRepository : IRepository<Tag>
    {
       
    }
    public class MovieTagRepository 
    {
        private CinemaContext _cinemaContext;

        public MovieTagRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }


        
        
    }
}

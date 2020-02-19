using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IProjectionsRepository : IRepository<Projection> 
    {
        IEnumerable<Projection> GetByAuditoriumId(int salaId);
        Task<IEnumerable<Projection>> FilteringProjections(object obj);
        Task<IEnumerable<Projection>> FilteringProjectionsByCinemas();
        Task<IEnumerable<Projection>> FilteringProjectionsByAuditoriums();
        Task<IEnumerable<Projection>> FilteringProjectionsByMovies();
        Task<IEnumerable<Projection>> FilteringProjectionsByDateTime();
    }

    public class ProjectionsRepository : IProjectionsRepository
    {
        private CinemaContext _cinemaContext;

        public ProjectionsRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public Projection Delete(object id)
        {
            Projection existing = _cinemaContext.Projections.Find(id);
            var result = _cinemaContext.Projections.Remove(existing).Entity;

            return result;
        }

        public async Task<IEnumerable<Projection>> GetAll()
        {
            var data = await _cinemaContext.Projections.Include(x => x.Movie).Include(x => x.Auditorium).ToListAsync();
            
            return data;           
        }

        public async Task<Projection> GetByIdAsync(object id)
        {
            return await _cinemaContext.Projections.FindAsync(id);
        }

        public IEnumerable<Projection> GetByAuditoriumId(int auditoriumId)
        {
            var projectionsData = _cinemaContext.Projections.Where(x => x.AuditoriumId == auditoriumId);

            return projectionsData;
        }

        public Projection Insert(Projection obj)
        {
            var data = _cinemaContext.Projections.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Projection Update(Projection obj)
        {
            var updatedEntry = _cinemaContext.Projections.Attach(obj).Entity;
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry;
        }

         public Task<IEnumerable<Projection>> FilteringProjections(object obj)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Projection>> FilteringProjectionsByCinemas()
        {
            var filteringProjections = await _cinemaContext.Projections
                .Include(x => x.Movie)
                .Include(x => x.Auditorium)
                .OrderByDescending(x => x.Auditorium.Cinema.Name)
                .ToListAsync();

            return filteringProjections;
        }

        public async Task<IEnumerable<Projection>> FilteringProjectionsByAuditoriums()
        {
            var filteringProjections = await _cinemaContext.Projections
                .Include(x => x.Movie)
                .Include(x => x.Auditorium)
                .OrderByDescending(x => x.Auditorium.Name)
                .ToListAsync();

            return filteringProjections;
        }

        public async Task<IEnumerable<Projection>> FilteringProjectionsByMovies()
        {
            var filteringProjections = await _cinemaContext.Projections
                .Include(x => x.Movie)
                .Include(x => x.Auditorium)
                .OrderByDescending(x => x.Movie.Title)
                .ToListAsync();

            return filteringProjections;
        }

        public async Task<IEnumerable<Projection>> FilteringProjectionsByDateTime()
        {
            var filteringProjections = await _cinemaContext.Projections
                .Include(x => x.Movie)
                .Include(x => x.Auditorium)
                .OrderByDescending(x => x.DateTime)
                .ToListAsync();

            return filteringProjections;
        }
    }
}

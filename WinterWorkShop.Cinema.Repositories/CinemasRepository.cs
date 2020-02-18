using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface ICinemasRepository : IRepository<Data.Cinema> { }

    public class CinemasRepository : ICinemasRepository
    {
        private CinemaContext _cinemaContext;

        public CinemasRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public Data.Cinema Delete(object id)
        {
            Data.Cinema existing = _cinemaContext.Cinemas.Find(id);
            var result = _cinemaContext.Cinemas.Remove(existing);

            return result.Entity;
        }

        public async Task<IEnumerable<Data.Cinema>> GetAll()
        {
            var data = await _cinemaContext.Cinemas.ToListAsync();

            return data;
        }

        public async Task<Data.Cinema> GetByIdAsync(object id)
        {
            var data = await _cinemaContext.Cinemas.FindAsync(id);

            return data;
        }

        public Data.Cinema Insert(Data.Cinema obj)
        {
            var data = _cinemaContext.Cinemas.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Data.Cinema Update(Data.Cinema obj)
        {
            var updatedEntry = _cinemaContext.Cinemas.Attach(obj);
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry.Entity;
        }
    }
}

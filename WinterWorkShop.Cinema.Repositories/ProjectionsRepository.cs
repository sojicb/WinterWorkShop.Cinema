﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IProjectionsRepository : IRepository<Projection> { }
    public class ProjectionsRepository : IProjectionsRepository
    {
        private CinemaContext _cinemaContext;

        public ProjectionsRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public EntityEntry<Projection> Delete(object id)
        {
            Projection existing = _cinemaContext.Projections.Find(id);
            return _cinemaContext.Projections.Remove(existing);
        }

        public async Task<IEnumerable<Projection>> GetAll()
        {
            return await _cinemaContext.Projections.ToListAsync();
        }

        public async Task<Projection> GetByIdAsync(object id)
        {
            return await _cinemaContext.Projections.FindAsync(id);
        }

        public EntityEntry<Projection> Insert(Projection obj)
        {
            return _cinemaContext.Projections.Add(obj);
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public EntityEntry<Projection> Update(Projection obj)
        {
            var updatedEntry = _cinemaContext.Projections.Attach(obj);
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry;
        }
    }
}
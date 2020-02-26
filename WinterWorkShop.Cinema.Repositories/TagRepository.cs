using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Repositories
{
	public interface ITagRepository : IRepository<Tag>
	{

	}

	public class TagRepository : ITagRepository
	{
		private readonly CinemaContext _cinemaContext;

		public TagRepository(CinemaContext cinemaContext)
		{
			_cinemaContext = cinemaContext;
		}

		public Tag Delete(object id)
		{
			Tag existing = _cinemaContext.Tags.Find(id);

			if (existing == null)
			{
				return null;
			}

			var result = _cinemaContext.Tags.Remove(existing);

			return result.Entity;
		}


		public async Task<IEnumerable<Tag>> GetAll()
		{
			return await _cinemaContext.Tags.ToListAsync();
		}


		public async Task<Tag> GetByIdAsync(object id)
		{
			var data = await _cinemaContext.Tags.SingleOrDefaultAsync(x=>x.Id.Equals((int)id));

			return data;
		}

		public Tag Insert(Tag obj)
		{
			var data = _cinemaContext.Tags.Add(obj).Entity;

			return data;
		}

		public void Save()
		{
			_cinemaContext.SaveChanges();
		}

		public Tag Update(Tag obj)
		{
			var updatedEntry = _cinemaContext.Tags.Attach(obj).Entity;
			_cinemaContext.Entry(obj).State = EntityState.Modified;

			return updatedEntry;
		}
	}
}

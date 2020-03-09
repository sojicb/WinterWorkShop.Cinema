using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Repositories
{
	public interface IReservationRepository : IRepository<Reservation>
	{

	}
	public class ReservationRepository : IReservationRepository
	{
		private CinemaContext _cinemaContext;

		public ReservationRepository(CinemaContext cinemaContext)
		{
			_cinemaContext = cinemaContext;
		}

		public Reservation Delete(object id)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<Reservation>> GetAll()
		{
			var data = await _cinemaContext.Reservations.Include(x => x.Projection).Include(x => x.User).Include(x => x.SeatReservation).ToListAsync();

			return data;
		}

		public Task<Reservation> GetByIdAsync(object id)
		{
			throw new NotImplementedException();
		}

		public Reservation Insert(Reservation obj)
		{
			var data = _cinemaContext.Reservations.Add(obj).Entity;

			return data;
		}

		public void Save()
		{
			_cinemaContext.SaveChanges();
		}

		public Reservation Update(Reservation obj)
		{
			throw new NotImplementedException();
		}
	}
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Repositories
{
	public interface ISeatReservationRepository : IRepository<SeatReservation>
	{

	}
	public class SeatReservationRepository : ISeatReservationRepository
	{
		private readonly CinemaContext _cinemaContext;

		public SeatReservationRepository(CinemaContext cinemaContext)
		{
			_cinemaContext = cinemaContext;
		}

		public SeatReservation Delete(object id)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<SeatReservation>> GetAll()
		{
			var data = await _cinemaContext.SeatReservations.Include(x => x.Seat).Include(x => x.Reservation).ToListAsync();

			return data;
		}

		public Task<SeatReservation> GetByIdAsync(object id)
		{
			throw new NotImplementedException();
		}

		public SeatReservation Insert(SeatReservation obj)
		{
			var data = _cinemaContext.SeatReservations.Add(obj).Entity;

			return data;
		}

		public void Save()
		{
			throw new NotImplementedException();
		}

		public SeatReservation Update(SeatReservation obj)
		{
			throw new NotImplementedException();
		}
	}
}

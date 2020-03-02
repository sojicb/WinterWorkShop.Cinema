using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
	public interface ISeatReservationService
	{
		Task<IEnumerable<SeatReservationDomainModel>> GetAllAsync();
	}
}

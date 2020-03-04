using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
	public interface IReservationService
	{
		Task<IEnumerable<ReservationDomainModel>> GetAllAsync();
		Task<ReservationDomainModel> CreateReservation(ReservationDomainModel domainModel);
		Task<ReservationDomainModel> GetByIdAsync(Guid id);
	}
}

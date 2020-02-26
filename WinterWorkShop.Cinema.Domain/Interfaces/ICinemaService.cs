using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface ICinemaService
    {
        Task<IEnumerable<CinemaDomainModel>> GetAllAsync();
        Task<CinemaDomainModel> GetCinemaByIdAsync(int id);
        Task<CreateCinemaResultModel> AddCinema(CinemaDomainModel newCinema);
        Task<CinemaDomainModel> UpdateCinema(CinemaDomainModel updateCinema);
        Task<DeleteCinemaDomainModel> DeleteCinema(int id);
    }
}

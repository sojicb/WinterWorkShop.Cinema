using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IAuditoriumService
    {
        Task<IEnumerable<AuditoriumDomainModel>> GetAllAsync();

        Task<CreateAuditoriumResultModel> CreateAuditorium(AuditoriumDomainModel domainModel, int numberOfRows, int numberOfSeats);
        Task<AuditoriumDomainModel> GetAuditoriumByIdAsync(int id);
       
        Task<AuditoriumDomainModel> UpdateAuditorium(AuditoriumDomainModel auditoriumToUpdate);
        Task<AuditoriumDomainModel> deleteAuditorium(Guid id);
    }
}

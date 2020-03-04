using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IProjectionService
    {
        Task<IEnumerable<ProjectionDomainModel>> GetAllAsync();
        Task<CreateProjectionResultModel> CreateProjection(ProjectionDomainModel domainModel);
        Task<ProjectionDomainModel> GetProjectionByIdAsync(Guid id);
        Task<ProjectionDomainModel> UpdateProjection(ProjectionDomainModel movieToUpdate);
        ProjectionDomainModel DeleteProjection(Guid id);
        Task<IEnumerable<ProjectionDomainModel>> FilterProjections(int? cinemaId, int? auditoriumId, Guid? movieId, DateTime? dateFrom, DateTime? dateTo);
    }
}

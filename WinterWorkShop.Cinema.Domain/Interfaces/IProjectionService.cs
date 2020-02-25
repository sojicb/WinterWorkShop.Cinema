﻿using System;
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
        Task<IEnumerable<ProjectionDomainModel>> FilterProjections(object obj);
        Task<IEnumerable<ProjectionDomainModel>> FilterProjectionsByCinemas();
        Task<IEnumerable<ProjectionDomainModel>> FilterProjectionsByAuditoriums();
        Task<IEnumerable<ProjectionDomainModel>> FilterProjectionsByMovies();
        Task<IEnumerable<ProjectionDomainModel>> FilterProjectionsBySpecificTime();
        Task<ProjectionDomainModel> GetProjectionByIdAsync(Guid id);
        Task<ProjectionDomainModel> UpdateProjection(ProjectionDomainModel movieToUpdate);
        ProjectionDomainModel DeleteProjection(Guid id);
    }
}

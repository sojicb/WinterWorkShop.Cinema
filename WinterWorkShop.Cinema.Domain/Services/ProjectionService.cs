using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class ProjectionService : IProjectionService
    {
        private readonly IProjectionsRepository _projectionsRepository;
        
        public ProjectionService(IProjectionsRepository projectionsRepository)
        {
            _projectionsRepository = projectionsRepository;
        }

        public async Task<IEnumerable<ProjectionDomainModel>> GetAllAsync()
        {
            var data = await _projectionsRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();
            ProjectionDomainModel model;
            foreach (var item in data)
            {
                model = new ProjectionDomainModel
                {
                    Id = item.Id,
                    MovieId = item.MovieId,
                    AuditoriumId = item.AuditoriumId,
                    ProjectionTime = item.DateTime,
                    MovieTitle = item.Movie.Title,
                    AditoriumName = item.Auditorium.Name
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<CreateProjectionResultModel> CreateProjection(ProjectionDomainModel domainModel)
        {
            int projectionTime = 3;

            var projectionsAtSameTime = _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId)
                .Where(x => x.DateTime < domainModel.ProjectionTime.AddHours(projectionTime) && x.DateTime > domainModel.ProjectionTime.AddHours(-projectionTime))
                .ToList();

            if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0)
            {
                return new CreateProjectionResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTIONS_AT_SAME_TIME
                };
            }

            var newProjection = new Data.Projection
            {
                MovieId = domainModel.MovieId,
                AuditoriumId = domainModel.AuditoriumId,
                DateTime = domainModel.ProjectionTime
            };

            var insertedProjection = _projectionsRepository.Insert(newProjection);

            if (insertedProjection == null)
            {
                return new CreateProjectionResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_CREATION_ERROR
                };
            }

            _projectionsRepository.Save();
            CreateProjectionResultModel result = new CreateProjectionResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projection = new ProjectionDomainModel
                {
                    Id = insertedProjection.Id,
                    AuditoriumId = insertedProjection.AuditoriumId,
                    MovieId = insertedProjection.MovieId,
                    ProjectionTime = insertedProjection.DateTime
                }
            };

            return result;
        }

        public Task<IEnumerable<ProjectionDomainModel>> FilterProjections(object obj)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProjectionDomainModel>> FilterProjectionsByCinemas()
        {
            var data = await _projectionsRepository.FilteringProjectionsByCinemas();

            if (data == null)
            {
                return null;
            }

            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();
            ProjectionDomainModel model;
            foreach (var item in data)
            {
                model = new ProjectionDomainModel
                {
                   AuditoriumId = item.AuditoriumId,
                   Id = item.Id,
                   MovieId = item.MovieId,
                   ProjectionTime = item.DateTime,
                   
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<IEnumerable<ProjectionDomainModel>> FilterProjectionsByAuditoriums()
        {
            var data = await _projectionsRepository.FilteringProjectionsByAuditoriums();

            if (data == null)
            {
                return null;
            }

            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();
            ProjectionDomainModel model;
            foreach (var item in data)
            {
                model = new ProjectionDomainModel
                {
                    AuditoriumId = item.AuditoriumId,
                    Id = item.Id,
                    MovieId = item.MovieId,
                    ProjectionTime = item.DateTime
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<IEnumerable<ProjectionDomainModel>> FilterProjectionsByMovies()
        {
            var data = await _projectionsRepository.FilteringProjectionsByMovies();

            if (data == null)
            {
                return null;
            }

            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();
            ProjectionDomainModel model;
            foreach (var item in data)
            {
                model = new ProjectionDomainModel
                {
                    AuditoriumId = item.AuditoriumId,
                    Id = item.Id,
                    MovieId = item.MovieId,
                    ProjectionTime = item.DateTime
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<IEnumerable<ProjectionDomainModel>> FilterProjectionsBySpecificTime()
        {
            var data = await _projectionsRepository.FilteringProjectionsByDateTime();

            if (data == null)
            {
                return null;
            }

            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();
            ProjectionDomainModel model;
            foreach (var item in data)
            {
                model = new ProjectionDomainModel
                {
                    AuditoriumId = item.AuditoriumId,
                    Id = item.Id,
                    MovieId = item.MovieId,
                    ProjectionTime = item.DateTime
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<ProjectionDomainModel> GetProjectionByIdAsync(Guid id)
        {
            var data = await _projectionsRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            ProjectionDomainModel  domainModel = new ProjectionDomainModel
            {
                Id = data.Id,
                AuditoriumId  = data.AuditoriumId,
                MovieId = data.MovieId,
                ProjectionTime = data.DateTime
            };

            return domainModel;
        }

        public async Task<ProjectionDomainModel> UpdateProjection(ProjectionDomainModel updateProjection)
        {
            Projection projection = new Projection()
            {
              Id = updateProjection.Id,
              AuditoriumId = updateProjection.AuditoriumId,
              MovieId = updateProjection.MovieId,
              DateTime = updateProjection.ProjectionTime

            };

            var data = _projectionsRepository.Update(projection);

            if (data == null)
            {
                return null;
            }
            _projectionsRepository.Save();

            ProjectionDomainModel domainModel = new ProjectionDomainModel()
            {
                Id = data.Id,
                AuditoriumId = data.AuditoriumId,
                MovieId = data.MovieId,
                ProjectionTime = data.DateTime
            };

            return domainModel;
        }

        public ProjectionDomainModel DeleteProjection(Guid id)
        {
            var data = _projectionsRepository.Delete(id);

            if(data == null)
            {
                return null;
            }

            _projectionsRepository.Save();

            ProjectionDomainModel domainModel = new ProjectionDomainModel
            {
                Id = data.MovieId,
                AuditoriumId = data.AuditoriumId,
                ProjectionTime = data.DateTime,
                MovieId = data.MovieId,

            };
            
            return domainModel;
        }
    }
}

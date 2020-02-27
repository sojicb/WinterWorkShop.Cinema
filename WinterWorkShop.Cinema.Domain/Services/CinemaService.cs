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
    public class CinemaService : ICinemaService
    {
        private readonly ICinemasRepository _cinemasRepository;
        private readonly IAuditoriumsRepository _auditoriumsRepository;
        private readonly IAuditoriumService _auditoriumService;

        public CinemaService(ICinemasRepository cinemasRepository, IAuditoriumsRepository auditoriumsRepository, IAuditoriumService auditoriumService)
        {
            _cinemasRepository = cinemasRepository;
            _auditoriumsRepository = auditoriumsRepository;
            _auditoriumService = auditoriumService;
        }

        public async Task<CreateCinemaResultModel> AddCinema(CinemaDomainModel newCinema)
        {
            Data.Cinema cinemaToCreate = new Data.Cinema()
            {
             Name = newCinema.Name
            };

            Data.Cinema insertedCinema = _cinemasRepository.Insert(cinemaToCreate);
            if (insertedCinema == null)
            {
                return new CreateCinemaResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.CINEMA_CREATION_ERROR
                };
            }

            _cinemasRepository.Save();

            CreateCinemaResultModel resultModel = new CreateCinemaResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Cinema = new CinemaDomainModel
                {
                    Id = insertedCinema.Id,
                    Name = insertedCinema.Name
                }
            };

            return resultModel;
        }

        public async Task<DeleteCinemaDomainModel> DeleteCinema(int id)
        {
            var cinema = await _cinemasRepository.GetAll();

            var auditoriums = cinema.SelectMany(x => x.Auditoriums.Where(x => x.CinemaId.Equals(id))).ToList();

            if(auditoriums == null)
            {
                return null;
            }

            foreach (var audit in auditoriums)
            {
                var auditorium = await _auditoriumService.DeleteAuditorium(audit.Id);

                if (!auditorium.IsSuccessful)
                {
                    return new DeleteCinemaDomainModel
                    {
                        IsSuccessful = false,
                        ErrorMessage = Messages.PROJECTION_IN_FUTURE_ON_CINEMA_DELETE
                    };
                }
            }

            var data = _cinemasRepository.Delete(id);

            if (data == null)
            {
                return null;
            }

            _cinemasRepository.Save();

            DeleteCinemaDomainModel domainModel = new DeleteCinemaDomainModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Cinema = new CinemaDomainModel
                {
                    Id = data.Id,
                    Name = data.Name
                }
            };

            return domainModel;
        }

        public async Task<IEnumerable<CinemaDomainModel>> GetAllAsync()
        {
            var data = await _cinemasRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<CinemaDomainModel> result = new List<CinemaDomainModel>();
            CinemaDomainModel model;
            foreach (var item in data)
            {
                model = new CinemaDomainModel
                {
                    Id = item.Id,
                    Name = item.Name
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<CinemaDomainModel> GetCinemaByIdAsync(int id)
        {
            var data = await _cinemasRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            CinemaDomainModel domainModel = new CinemaDomainModel
            {
                Id = data.Id,
                Name = data.Name
            };

            return domainModel;
        }

        public async Task<CinemaDomainModel> UpdateCinema(CinemaDomainModel updateCinema)
        {
            Data.Cinema cinema = new Data.Cinema()
            {
                Id = updateCinema.Id,
                Name = updateCinema.Name
            };

            var data = _cinemasRepository.Update(cinema);

            if (data == null)
            {
                return null;
            }
            _cinemasRepository.Save();

            CinemaDomainModel domainModel = new CinemaDomainModel()
            {
                Id = data.Id,
                Name = data.Name
            };

            return domainModel;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class AuditoriumService : IAuditoriumService
    {
        private readonly IAuditoriumsRepository _auditoriumsRepository;
        private readonly ICinemasRepository _cinemasRepository;
        private readonly ISeatsRepository _seatsRepository;
        private readonly IProjectionsRepository _projectionsRepository;

        public AuditoriumService(IAuditoriumsRepository auditoriumsRepository, ICinemasRepository cinemasRepository, ISeatsRepository seatsRepository, IProjectionsRepository projectionsRepository)
        {
            _auditoriumsRepository = auditoriumsRepository;
            _cinemasRepository = cinemasRepository;
            _seatsRepository = seatsRepository;
            _projectionsRepository = projectionsRepository;
        }

        public async Task<IEnumerable<AuditoriumDomainModel>> GetAllAsync()
        {
            var data = await _auditoriumsRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<AuditoriumDomainModel> result = new List<AuditoriumDomainModel>();
            List<SeatDomainModel> seats = new List<SeatDomainModel>();
            AuditoriumDomainModel model;

            foreach (var item in data)
            {
                foreach (var seat in item.Seats)
                {
                    seats.Add(new SeatDomainModel
                    {
                        Id = seat.Id,
                        AuditoriumId = seat.AuditoriumId,
                        Number = seat.Number,
                        Row = seat.Row
                    });
                }

                model = new AuditoriumDomainModel
                {
                    Id = item.Id,
                    CinemaId = item.CinemaId,
                    Name = item.Name,
                    SeatsList = seats.Where(x => x.AuditoriumId.Equals(item.Id)).ToList()
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<CreateAuditoriumResultModel> CreateAuditorium(AuditoriumDomainModel domainModel, int numberOfRows, int numberOfSeats)
        {
            var cinema = await _cinemasRepository.GetByIdAsync(domainModel.CinemaId);
            if (cinema == null)
            {
                return new CreateAuditoriumResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.AUDITORIUM_UNVALID_CINEMAID
                };
            }

            var auditorium = await _auditoriumsRepository.GetByAuditName(domainModel.Name, domainModel.CinemaId);
            var sameAuditoriumName = auditorium.ToList();
            if (sameAuditoriumName != null && sameAuditoriumName.Count > 0)
            {
                return new CreateAuditoriumResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.AUDITORIUM_SAME_NAME
                };
            }

            Auditorium newAuditorium = new Auditorium
            {
                Name = domainModel.Name,
                CinemaId = domainModel.CinemaId,
            };

            newAuditorium.Seats = new List<Seat>();

            for (int i = 1; i <= numberOfRows; i++)
            {
                for (int j = 1; j <= numberOfSeats; j++)
                {
                    Seat newSeat = new Seat()
                    {
                        Row = i,
                        Number = j
                    };

                    newAuditorium.Seats.Add(newSeat);
                }
            }

            Auditorium insertedAuditorium = _auditoriumsRepository.Insert(newAuditorium);
            if (insertedAuditorium == null)
            {
                return new CreateAuditoriumResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.AUDITORIUM_CREATION_ERROR
                };
            }

            CreateAuditoriumResultModel resultModel = new CreateAuditoriumResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Auditorium = new AuditoriumDomainModel
                {
                    Id = insertedAuditorium.Id,
                    Name = insertedAuditorium.Name,
                    CinemaId = insertedAuditorium.CinemaId,
                    SeatsList = new List<SeatDomainModel>()
                }
            };

            foreach (var item in insertedAuditorium.Seats)
            {
                resultModel.Auditorium.SeatsList.Add(new SeatDomainModel
                {
                    AuditoriumId = insertedAuditorium.Id,
                    Id = item.Id,
                    Number = item.Number,
                    Row = item.Row
                });
            }
            _auditoriumsRepository.Save();
            return resultModel;
        }

        public async Task<AuditoriumDomainModel> GetAuditroiumByIdAsync(int id)
        {
            var data = await _auditoriumsRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            List<SeatDomainModel> seats = new List<SeatDomainModel>();

            foreach (var seat in data.Seats)
            {
                seats.Add(new SeatDomainModel
                {
                    Id = seat.Id,
                    AuditoriumId = seat.AuditoriumId,
                    Number = seat.Number,
                    Row = seat.Row
                });
            }

            AuditoriumDomainModel domainModel = new AuditoriumDomainModel
            {
                Id = data.Id,
                CinemaId = data.CinemaId,
                Name = data.Name,
                SeatsList = seats
            };

            return domainModel;

        }

        public async Task<AuditoriumDomainModel> UpdateAuditorium(AuditoriumDomainModel auditoriumToUpdate)
        {

            Auditorium auditroium = new Auditorium()
            {
                Id = auditoriumToUpdate.Id,
                CinemaId = auditoriumToUpdate.CinemaId,
                Name = auditoriumToUpdate.Name,
            };

            var data = _auditoriumsRepository.Update(auditroium);

            if (data == null)
            {
                return null;
            }
            _auditoriumsRepository.Save();

            AuditoriumDomainModel domainModel = new AuditoriumDomainModel()
            {
                Id = data.Id,
                CinemaId = data.CinemaId,
                Name = data.Name
            };

            return domainModel;
        }

        public async Task<DeleteAuditoriumDomainModel> DeleteAuditorium(int id)
        {
            var audit = await _auditoriumsRepository.GetByIdAsync(id);

            var seats = audit.Seats.ToList();

            var projections = audit.Projections.ToList();

            foreach (var projection in projections)
            {
                if (projection.DateTime > DateTime.Now)
                {
                    return new DeleteAuditoriumDomainModel
                    {
                        ErrorMessage = Messages.PROJECTION_IN_FUTURE_ON_CINEMA_DELETE,
                        IsSuccessful = false
                    };
                }
            }

            foreach(var projection in projections)
            {
                _projectionsRepository.Delete(projection.Id);
            }

            foreach (var seat in seats)
            {
                var seatResult = _seatsRepository.Delete(seat.Id);

                if(seatResult == null)
                {
                    return null;
                }
            }

            var data = _auditoriumsRepository.Delete(id);


            if (data == null)
            {
                return null;
            }

            _auditoriumsRepository.Save();

            DeleteAuditoriumDomainModel domainModel = new DeleteAuditoriumDomainModel
            {
               IsSuccessful = true,
               ErrorMessage = null,
               Auditorium = new AuditoriumDomainModel
               {
                   Id = data.Id,
                   Name = data.Name,
                   CinemaId = data.CinemaId,
               }
            };

            return domainModel;
        }

        public async Task<IEnumerable<AuditoriumDomainModel>> GetAllByCinemaId(int id)
        {
            var data = await _auditoriumsRepository.GetAll();

            if(data == null)
            {
                return null;
            }
            var audits = data.Where(x => x.CinemaId.Equals(id));

            List<AuditoriumDomainModel> models = new List<AuditoriumDomainModel>();

            foreach(var audit in audits)
            {
                models.Add(new AuditoriumDomainModel
                {
                    Id = audit.Id,
                    CinemaId = audit.CinemaId,
                    Name = audit.Name
                });
            }
            return models;
        }
    }
}

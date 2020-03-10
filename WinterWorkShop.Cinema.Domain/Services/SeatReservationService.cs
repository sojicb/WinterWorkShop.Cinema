using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
	public class SeatReservationService : ISeatReservationService
	{
		private readonly ISeatReservationRepository _seatReservationRepository;
		private readonly IAuditoriumsRepository _auditoriumsRepository;
		private readonly ISeatsRepository _seatRepository;

		public SeatReservationService(ISeatReservationRepository seatReservationRepository, IAuditoriumsRepository auditoriumsRepository, ISeatsRepository seatRepository)
		{
			_seatReservationRepository = seatReservationRepository;
			_auditoriumsRepository = auditoriumsRepository;
			_seatRepository = seatRepository;
		}

		public async Task<IEnumerable<SeatReservationDomainModel>> GetAllAsync()
		{
			var data = await _seatReservationRepository.GetAll();

			if(data == null)
			{
				return null;
			}

			List<SeatReservationDomainModel> seatReservations = new List<SeatReservationDomainModel>();

			foreach(var seatReservation in data)
			{
				seatReservations.Add(new SeatReservationDomainModel
				{
					ReservationId = seatReservation.ReservationId,
					SeatId = seatReservation.SeatId,
					Seat = new SeatDomainModel
					{
						Id = seatReservation.Seat.Id,
						AuditoriumId = seatReservation.Seat.AuditoriumId,
						Number = seatReservation.Seat.Number,
						Row = seatReservation.Seat.Row
					},
					Reservation = new ReservationDomainModel
					{
						Id = seatReservation.ReservationId
					}
				});
			}

			return seatReservations;
		}

		public async Task<IEnumerable<SeatReservationDomainModel>> InsertResevedSeats(InsertSeatReservationModel seatReservation)
		{
			SeatReservation data = new SeatReservation();

			List<SeatReservation> insertedReservations = new List<SeatReservation>();

			List<SeatReservationDomainModel> seatReservations = new List<SeatReservationDomainModel>();

			foreach (var seat in seatReservation.SeatIds)
			{
				SeatReservation model = new SeatReservation
				{
					ReservationId = seatReservation.ReservationId,
					ProjectionTime = seatReservation.ProjectionTime,
					SeatId = seat
				};

				data = _seatReservationRepository.Insert(model);
				
				if(data == null)
				{
					return null;
				}

				insertedReservations.Add(data);
			}

			//_seatReservationRepository.Save();

			foreach(var item in insertedReservations)
			{
				seatReservations.Add(new SeatReservationDomainModel
				{
					ProjectionTime = item.ProjectionTime,
					ReservationId = item.ReservationId,
					SeatId = item.SeatId
				});
			}
			return seatReservations;
		}

		public async Task<SeatReservationValidationDomainModel> ValidateSeat(SeatReservationDomainModel model)
		{
			var seatReservations = await _seatReservationRepository.GetAll();

			if(seatReservations == null)
			{
				return null;
			}

			foreach(var seat in seatReservations)
			{
				if(seat.ProjectionTime.Equals(model.ProjectionTime) && seat.SeatId.Equals(model.SeatId))
				{
					return new SeatReservationValidationDomainModel
					{
						IsSuccessful = false,
						ErrorMessage = Messages.SEAT_ALREADY_RESERVED,
						SeatReservation = new SeatReservationDomainModel
						{
							SeatId = seat.SeatId
						}
					};
				}
			}

			return new SeatReservationValidationDomainModel
			{
				IsSuccessful = true,
				ErrorMessage = null
			};
		}

		public async Task<ValidateSeatDomainModel> HandleSeatReservation(SeatValidationDomainModel model)
		{
			List<SeatDomainModel> seats = new List<SeatDomainModel>();

			SeatReservationDomainModel domainModel = new SeatReservationDomainModel();

			var seatsData = _seatRepository.GetAll().Result.Where(x => model.SeatIds.Contains(x.Id)).ToList();

			foreach(var seat in seatsData)
			{
				seats.Add(new SeatDomainModel
				{
					Id = seat.Id,
					AuditoriumId = seat.AuditoriumId,
					Number = seat.Number,
					Row = seat.Row
				});
			}

			foreach (var seat in model.SeatIds)
			{
				domainModel = new SeatReservationDomainModel
				{
					SeatId = seat,
					ProjectionTime = model.ProjectionTime
				};

				var data = await ValidateSeat(domainModel);

				if (!data.IsSuccessful)
				{
					return new ValidateSeatDomainModel
					{
						IsSuccessful = false,
						ErrorMessage = data.ErrorMessage,
						Seat = new SeatDomainModel
						{
							Id = seat
						}
					};
				}
			}

			seats = seats.OrderBy(x => x.Number).ToList();

			if (seats.Select((x, y) => x.Number - y).Distinct().Skip(1).Any())
			{
				return new ValidateSeatDomainModel
				{
					IsSuccessful = false,
					ErrorMessage = Messages.SEATS_NOT_CONSECUTIVE
				};
			}


			for (int i = 0; i < seats.Count - 1; i++)
			{
				if (!seats.ElementAt(i).Row.Equals(seats.ElementAt(i + 1).Row))
				{
					return new ValidateSeatDomainModel
					{
						IsSuccessful = false,
						ErrorMessage = Messages.SEAT_IN_WRONG_ROW
					};
				}
			}

			return new ValidateSeatDomainModel
			{
				IsSuccessful = true,
				ErrorMessage = null
			};
		}
	}
}

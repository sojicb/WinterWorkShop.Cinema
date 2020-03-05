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

		public SeatReservationService(ISeatReservationRepository seatReservationRepository, IAuditoriumsRepository auditoriumsRepository)
		{
			_seatReservationRepository = seatReservationRepository;
			_auditoriumsRepository = auditoriumsRepository;
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

			foreach (var seat in seatReservation.Seats)
			{
				SeatReservation model = new SeatReservation
				{
					ReservationId = seatReservation.ReservationId,
					ProjectionTime = seatReservation.ProjectionTime,
					SeatId = seat.Id
				};

				data = _seatReservationRepository.Insert(model);
				
				if(data == null)
				{
					return null;
				}

				insertedReservations.Add(data);
			}

			_seatReservationRepository.Save();

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
			var audit = await _auditoriumsRepository.GetByIdAsync(model.AuditoriumId);

			if (audit == null)
			{
				return null;
			}

			//All Audit Seats
			List<SeatDomainModel> seats = new List<SeatDomainModel>();

			SeatReservationDomainModel domainModel = new SeatReservationDomainModel();

			foreach (var seat in audit.Seats)
			{
				seats.Add(new SeatDomainModel
				{
					Id = seat.Id,
					AuditoriumId = seat.AuditoriumId,
					Number = seat.Number,
					Row = seat.Row
				});
			}

			foreach (var seat in model.Seats)
			{
				domainModel = new SeatReservationDomainModel
				{
					SeatId = seat.Id,
					ProjectionTime = model.ProjectionTime
				};

				var data = await ValidateSeat(domainModel);

				if (!data.IsSuccessful)
				{
					return new ValidateSeatDomainModel
					{
						IsSuccessful = false,
						ErrorMessage = Messages.SEAT_ALREADY_RESERVED,
						Seat = seat
					};
				}
			}


			for (int i = 0; i < model.Seats.Count - 1; i++)
			{
				if(!model.Seats.ElementAt(i).Row.Equals(model.Seats.ElementAt(i + 1).Row))
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

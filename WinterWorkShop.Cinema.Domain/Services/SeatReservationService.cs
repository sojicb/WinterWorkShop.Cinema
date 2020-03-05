using System;
using System.Collections.Generic;
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

		public SeatReservationService(ISeatReservationRepository seatReservationRepository)
		{
			_seatReservationRepository = seatReservationRepository;
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
	}
}

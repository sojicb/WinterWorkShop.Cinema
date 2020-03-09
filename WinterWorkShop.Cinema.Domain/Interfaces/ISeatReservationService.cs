﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
	public interface ISeatReservationService
	{
		Task<IEnumerable<SeatReservationDomainModel>> GetAllAsync();
		Task<IEnumerable<SeatReservationDomainModel>> InsertResevedSeats(InsertSeatReservationModel seatReservation);
		Task<SeatReservationValidationDomainModel> ValidateSeat(SeatReservationDomainModel model);
		Task<ValidateSeatDomainModel> HandleSeatReservation(SeatValidationDomainModel model);
		Task<IEnumerable<SeatDomainModel>> GetReservedSeats(int auditoriumId);
	}
}

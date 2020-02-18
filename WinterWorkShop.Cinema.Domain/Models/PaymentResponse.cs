using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class PaymentResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }
    }
}

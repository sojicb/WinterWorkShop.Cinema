using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.API.Models
{
    public class PaymentResponseModel
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }
    }
}

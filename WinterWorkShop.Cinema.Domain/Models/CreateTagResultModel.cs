using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class CreateTagResultModel
    {
        public TagDomainModel Tag { get; set; }

        public bool IsSuccessful { get; set; }

        public string ErrorMessage { get; set; }

        

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RentCWeb.Models
{
    public class CarRent
    {
        [Required]
        public Reservation Reservation { get; set; }

        [Required]
        public Car Car { get; set; }

        public string StartDate { get; set; }
    
        public string EndDate { get; set; }
    }
}
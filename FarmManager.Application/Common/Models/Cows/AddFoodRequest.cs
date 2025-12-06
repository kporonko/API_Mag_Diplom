using FarmManager.Application.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Models.Cows
{
    public class AddFoodRequest
    {
        [Required]
        public int CowId { get; set; }

        [Range(0.1, 500.0)]
        public double AmountInKg { get; set; }

        [Range(0.01, 100000.0)]
        public decimal Cost { get; set; }

        public string? Period { get; set; }
    }
}

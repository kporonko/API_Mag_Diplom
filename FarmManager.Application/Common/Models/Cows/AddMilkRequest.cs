using FarmManager.Application.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Models.Cows
{
    public class AddMilkRequest
    {
        [Required]
        public int CowId { get; set; }

        [Range(0.1, 100.0)]
        public double AmountInLiters { get; set; }
        public decimal PricePerLiter { get; set; } 

        public string? Period { get; set; }
    }
}

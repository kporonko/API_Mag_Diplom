using FarmManager.Application.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Models.Cows
{
    public class CalculateWeightRequest
    {
        [Required]
        public int CowId { get; set; }

        [Required]
        public IFormFile Photo { get; set; } = null!;

        public string? Period { get; set; }
    }
}

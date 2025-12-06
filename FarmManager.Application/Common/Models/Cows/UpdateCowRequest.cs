using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Models.Cows
{
    public class UpdateCowRequest
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Breed { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }

        public bool IsFlaggedForSlaughter { get; set; }
    }
}

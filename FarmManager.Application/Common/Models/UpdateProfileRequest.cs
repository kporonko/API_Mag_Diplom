using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Models
{
    public class UpdateProfileRequest
    {
        public string FullName { get; set; } = string.Empty;

        // Настройки
        public decimal DefaultMilkPrice { get; set; }
        public decimal DefaultFoodPrice { get; set; }
        public decimal DefaultMeatPrice { get; set; }
        public string Language { get; set; } = "en";
    }
}

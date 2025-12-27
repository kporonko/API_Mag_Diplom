using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Domain.Entities
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string? FullName { get; set; }

        public decimal DefaultMilkPrice { get; set; } = 0m; // Цена за литр
        public decimal DefaultFoodPrice { get; set; } = 0m;
        public decimal DefaultMeatPrice { get; set; } = 0m;
        public string Language { get; set; } = "ua";
        public List<Cow> Cows { get; set; } = new List<Cow>();
    }
}
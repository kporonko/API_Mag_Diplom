using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Domain.Entities
{
    public class FoodConsumption
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double AmountInKg { get; set; }
        public decimal PricePerKg { get; set; }

        // --- Связь ---
        public int CowId { get; set; }
        public virtual Cow Cow { get; set; } = null!;
    }
}

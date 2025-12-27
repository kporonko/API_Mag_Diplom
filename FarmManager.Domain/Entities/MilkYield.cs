using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Domain.Entities
{
    public class MilkYield
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double AmountInLiters { get; set; }

        // Новое поле: Сколько денег это принесло на момент записи
        public decimal PricePerLiter { get; set; }

        public int CowId { get; set; }
        public virtual Cow Cow { get; set; } = null!;
    }
}

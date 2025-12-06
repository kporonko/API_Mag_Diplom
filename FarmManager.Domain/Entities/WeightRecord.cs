using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Domain.Entities
{
    public class WeightRecord
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double WeightInKg { get; set; }

        // Ссылка на фото. Сами фото храним в S3, Azure Blob или на диске,
        // но в БД только путь.
        public string PhotoUrl { get; set; }

        // --- Связь (Внешний ключ) ---
        public int CowId { get; set; }
        public virtual Cow Cow { get; set; } = null!; // null! - говорим компилятору, что EF это заполнит
    }
}

namespace FarmManager.Domain.Entities
{
    public class Cow
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }

        // --- ДОБАВЛЕНО: Привязка к пользователю ---
        public int ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;
        // -------------------------------------------

        // Связи
        public virtual ICollection<WeightRecord> WeightHistory { get; set; } = new List<WeightRecord>();
        public virtual ICollection<MilkYield> MilkHistory { get; set; } = new List<MilkYield>();
        public virtual ICollection<FoodConsumption> FoodHistory { get; set; } = new List<FoodConsumption>();
    }
}
namespace FarmManager.Application.Common.Models
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; }

        // Твои настройки
        public string Language { get; set; }
        public decimal DefaultMilkPrice { get; set; }
        public decimal DefaultFoodPrice { get; set; }
        public decimal DefaultMeatPrice { get; set; }
    }
}
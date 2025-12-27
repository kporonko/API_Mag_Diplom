namespace FarmManager.Application.Common.Models.Cows
{
    public class CowResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public bool IsFlaggedForSlaughter { get; set; }
        public double ProfitRatio { get; set; }
        public decimal TotalIncome { get; set; }
        public IEnumerable<WeightRecordDto> WeightHistory { get; set; } = new List<WeightRecordDto>();
        public IEnumerable<MilkYieldDto> MilkHistory { get; set; } = new List<MilkYieldDto>();
        public IEnumerable<FoodConsumptionDto> FoodHistory { get; set; } = new List<FoodConsumptionDto>();

        public class WeightRecordDto
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public double WeightInKg { get; set; }
            public string PhotoUrl { get; set; } = string.Empty;
        }

        public class MilkYieldDto
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public double AmountInLiters { get; set; }

            // --- НОВОЕ ПОЛЕ ---
            public decimal PricePerLiter { get; set; }
        }

        public class FoodConsumptionDto
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public double AmountInKg { get; set; }
            public decimal PricePerKg { get; set; }
        }
    }
}
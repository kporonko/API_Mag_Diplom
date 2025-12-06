namespace FarmManager.Application.Common.Models.Production
{
    // DTO для ответа аналитики
    public class AnalyticsResponse
    {
        public int CowId { get; set; }
        public bool IsFlaggedForSlaughter { get; set; }
        public double MilkToFoodRatio { get; set; }
        public decimal EstimatedMeatValue { get; set; }

        public decimal TotalMilkIncome { get; set; } // <-- НОВОЕ ПОЛЕ (Доходы)
        public decimal TotalUpkeepCost { get; set; } // (Расходы)
    }
}
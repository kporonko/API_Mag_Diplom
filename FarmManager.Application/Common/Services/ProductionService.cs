using FarmManager.Application.Common.Interfaces;
using FarmManager.Application.Common.Models.Cows;
using FarmManager.Application.Common.Models.Production;
using FarmManager.Application.Common.Repositories;
using FarmManager.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace FarmManager.Application.Services
{
    public enum AnalyticsPeriod
    {
        Week,
        Month,
        SixMonths,
        AllTime
    }

    public class ProductionService : IProductionService
    {
        private readonly ICowRepository _cowRepository;
        private readonly IWeightRecordRepository _weightRepository;
        private readonly IMilkYieldRepository _milkRepository;
        private readonly IFoodConsumptionRepository _foodRepository;
        private readonly IFlaskMlClient _flaskClient;
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public ProductionService(
            ICowRepository cowRepository,
            IWeightRecordRepository weightRepository,
            IMilkYieldRepository milkRepository,
            IFoodConsumptionRepository foodRepository,
            IFlaskMlClient flaskClient,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _cowRepository = cowRepository;
            _weightRepository = weightRepository;
            _milkRepository = milkRepository;
            _foodRepository = foodRepository;
            _flaskClient = flaskClient;
            _fileStorage = fileStorage;
            _unitOfWork = unitOfWork;
            _config = configuration;
        }

        public async Task<CowResponse.FoodConsumptionDto> AddFoodConsumptionAsync(int userId, AddFoodRequest request)
        {
            await ValidateCowOwnershipAsync(request.CowId, userId);

            var food = new FoodConsumption
            {
                CowId = request.CowId,
                Date = DateTime.UtcNow,
                AmountInKg = request.AmountInKg,
                PricePerKg = request.PricePerKg
            };

            await _foodRepository.AddAsync(food);
            await _unitOfWork.SaveChangesAsync();

            await RefreshAnalyticsAsync(request.CowId, userId, request.Period);

            return new CowResponse.FoodConsumptionDto
            {
                Id = food.Id,
                Date = food.Date,
                AmountInKg = food.AmountInKg,
                PricePerKg = food.PricePerKg
            };
        }

        public async Task<CowResponse.MilkYieldDto> AddMilkYieldAsync(int userId, AddMilkRequest request)
        {
            await ValidateCowOwnershipAsync(request.CowId, userId);

            var milk = new MilkYield
            {
                CowId = request.CowId,
                Date = DateTime.UtcNow,
                AmountInLiters = request.AmountInLiters,
                PricePerLiter = request.PricePerLiter
            };

            await _milkRepository.AddAsync(milk);
            await _unitOfWork.SaveChangesAsync();

            await RefreshAnalyticsAsync(request.CowId, userId, request.Period);

            return new CowResponse.MilkYieldDto
            {
                Id = milk.Id,
                Date = milk.Date,
                AmountInLiters = milk.AmountInLiters,
                PricePerLiter = milk.PricePerLiter
            };
        }

        public async Task<CowResponse.WeightRecordDto> CalculateWeightAsync(int userId, CalculateWeightRequest request)
        {
            await ValidateCowOwnershipAsync(request.CowId, userId);

            double weight = await _flaskClient.GetWeightFromPhotoAsync(request.Photo);
            string photoUrl = await _fileStorage.SaveFileAsync(request.Photo, $"cow_weights/cowId_{request.CowId}");

            var weightRecord = new WeightRecord
            {
                CowId = request.CowId,
                Date = DateTime.UtcNow,
                WeightInKg = weight,
                PhotoUrl = photoUrl
            };

            await _weightRepository.AddAsync(weightRecord);
            await _unitOfWork.SaveChangesAsync();

            await RefreshAnalyticsAsync(request.CowId, userId, request.Period);

            return new CowResponse.WeightRecordDto
            {
                Id = weightRecord.Id,
                Date = weightRecord.Date,
                WeightInKg = weightRecord.WeightInKg,
                PhotoUrl = photoUrl
            };
        }

        // --- ГЛАВНЫЙ МЕТОД АНАЛИТИКИ (Теперь чистый) ---
        public async Task<AnalyticsResponse> RunAnalyticsAsync(int cowId, int userId, AnalyticsPeriod period = AnalyticsPeriod.Month)
        {
            var cow = await _cowRepository.GetByIdWithHistoryAsync(cowId, userId);
            if (cow == null) throw new KeyNotFoundException("Корова не найдена или не принадлежит пользователю.");

            var startDate = GetAnalyticsStartDate(period);
            var (income, cost) = CalculateFinancials(cow, startDate);
            var roi = CalculateRoi(income, cost);

            var isFlaggedForSlaughter = CheckSlaughterCriteria(roi, cost);
            var meatValue = CalculateMeatValue(cow);

            return new AnalyticsResponse
            {
                CowId = cowId,
                IsFlaggedForSlaughter = isFlaggedForSlaughter,
                MilkToFoodRatio = roi,
                EstimatedMeatValue = meatValue,
                TotalMilkIncome = income,
                TotalUpkeepCost = cost
            };
        }

        // --- ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ (PRIVATE) ---

        private async Task ValidateCowOwnershipAsync(int cowId, int userId)
        {
            var exists = await _cowRepository.GetByIdAsync(cowId, userId);
            if (exists == null)
                throw new KeyNotFoundException("Корова не найдена или не принадлежит пользователю.");
        }

        private async Task RefreshAnalyticsAsync(int cowId, int userId, string periodString)
        {
            var period = Enum.TryParse<AnalyticsPeriod>(periodString, true, out var parsed)
                ? parsed
                : AnalyticsPeriod.Month;

            await RunAnalyticsAsync(cowId, userId, period);
        }

        private DateTime? GetAnalyticsStartDate(AnalyticsPeriod period)
        {
            return period switch
            {
                AnalyticsPeriod.Week => DateTime.UtcNow.AddDays(-7),
                AnalyticsPeriod.Month => DateTime.UtcNow.AddMonths(-1),
                AnalyticsPeriod.SixMonths => DateTime.UtcNow.AddMonths(-6),
                AnalyticsPeriod.AllTime => null,
                _ => DateTime.UtcNow.AddMonths(-1)
            };
        }

        private (decimal Income, decimal Cost) CalculateFinancials(Cow cow, DateTime? startDate)
        {
            var milkHistory = startDate.HasValue ? cow.MilkHistory.Where(m => m.Date >= startDate.Value) : cow.MilkHistory;
            var foodHistory = startDate.HasValue ? cow.FoodHistory.Where(f => f.Date >= startDate.Value) : cow.FoodHistory;

            decimal income = milkHistory.Sum(m => (decimal)m.AmountInLiters * m.PricePerLiter);
            decimal cost = foodHistory.Sum(f => (decimal)f.AmountInKg * f.PricePerKg);

            return (income, cost);
        }

        private double CalculateRoi(decimal income, decimal cost)
        {
            if (cost > 0) return (double)(income / cost);
            return income > 0 ? 100.0 : 0;
        }

        private bool CheckSlaughterCriteria(double roi, decimal totalCost)
        {
            double threshold = _config.GetValue<double>("FarmConfig:MilkToFoodRatioThreshold");
            // Если ROI ниже порога и мы тратим деньги на еду -> пора на мясо
            return roi < threshold && totalCost > 0;
        }

        private decimal CalculateMeatValue(Cow cow)
        {
            var latestWeight = cow.WeightHistory
                .OrderByDescending(w => w.Date)
                .FirstOrDefault()?.WeightInKg ?? 0;

            // Здесь предполагается, что User доступен через навигационное свойство
            var meatPrice = cow.ApplicationUser?.DefaultMeatPrice ?? 0;
            return (decimal)latestWeight * meatPrice;
        }
    }
}
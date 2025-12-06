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

        // --- ИСПРАВЛЕНО: Добавлен userId ---
        public async Task<CowResponse.FoodConsumptionDto> AddFoodConsumptionAsync(int userId, AddFoodRequest request)
        {
            // Проверяем существование и владение коровой
            // Теперь используем GetByIdAsync(request.CowId, userId)
            if (await _cowRepository.GetByIdAsync(request.CowId, userId) == null)
                throw new KeyNotFoundException("Корова не найдена или не принадлежит пользователю.");

            var food = new FoodConsumption
            {
                CowId = request.CowId,
                Date = DateTime.UtcNow,
                AmountInKg = request.AmountInKg,
                Cost = request.Cost
            };

            await _foodRepository.AddAsync(food);
            await _unitOfWork.SaveChangesAsync();

            // АВТОМАТИЧЕСКИЙ ПЕРЕСЧЕТ СТАТУСА "К ЗАБОЮ" ПОСЛЕ КОРМЛЕНИЯ
            var period = Enum.TryParse<AnalyticsPeriod>(request.Period, true, out var parsed)
                ? parsed
                : AnalyticsPeriod.Month;

            await RunAnalyticsAsync(request.CowId, userId, period);

            return new CowResponse.FoodConsumptionDto
            {
                Id = food.Id,
                Date = food.Date,
                AmountInKg = food.AmountInKg,
                Cost = food.Cost
            };
        }

        // --- ИСПРАВЛЕНО: Добавлен userId ---
        public async Task<CowResponse.MilkYieldDto> AddMilkYieldAsync(int userId, AddMilkRequest request)
        {
            // Проверяем существование и владение коровой
            // Теперь используем GetByIdAsync(request.CowId, userId)
            if (await _cowRepository.GetByIdAsync(request.CowId, userId) == null)
                throw new KeyNotFoundException("Корова не найдена или не принадлежит пользователю.");

            var milk = new MilkYield
            {
                CowId = request.CowId,
                Date = DateTime.UtcNow,
                AmountInLiters = request.AmountInLiters
            };

            await _milkRepository.AddAsync(milk);
            await _unitOfWork.SaveChangesAsync();

            // АВТОМАТИЧЕСКИЙ ПЕРЕСЧЕТ СТАТУСА "К ЗАБОЮ" ПОСЛЕ НАДОЯ
            var period = Enum.TryParse<AnalyticsPeriod>(request.Period, true, out var parsed)
                ? parsed
                : AnalyticsPeriod.Month;

            await RunAnalyticsAsync(request.CowId, userId, period);
            return new CowResponse.MilkYieldDto
            {
                Id = milk.Id,
                Date = milk.Date,
                AmountInLiters = milk.AmountInLiters
            };
        }

        // --- ИСПРАВЛЕНО: Добавлен userId ---
        public async Task<CowResponse.WeightRecordDto> CalculateWeightAsync(int userId, CalculateWeightRequest request)
        {
            // Проверяем существование и владение коровой
            // Теперь используем GetByIdAsync(request.CowId, userId)
            if (await _cowRepository.GetByIdAsync(request.CowId, userId) == null)
                throw new KeyNotFoundException("Корова не найдена или не принадлежит пользователю.");

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

            var period = Enum.TryParse<AnalyticsPeriod>(request.Period, true, out var parsed)
                ? parsed
                : AnalyticsPeriod.Month;

            await RunAnalyticsAsync(request.CowId, userId, period);

            return new CowResponse.WeightRecordDto
            {
                Id = weightRecord.Id,
                Date = weightRecord.Date,
                WeightInKg = weightRecord.WeightInKg,
                PhotoUrl = photoUrl
            };
        }

        // --- ИСПРАВЛЕНО: Добавлен userId ---
        public async Task<AnalyticsResponse> RunAnalyticsAsync(int cowId, int userId, AnalyticsPeriod period = AnalyticsPeriod.Month)
        {
            // Запрашиваем корову с историей и фильтром по владельцу
            // Теперь используем GetByIdWithHistoryAsync(cowId, userId)
            var cow = await _cowRepository.GetByIdWithHistoryAsync(cowId, userId);
            if (cow == null) throw new KeyNotFoundException("Корова не найдена или не принадлежит пользователю.");

            // 1. Определяем дату начала выборки
            DateTime? dateFrom = period switch
            {
                AnalyticsPeriod.Week => DateTime.UtcNow.AddDays(-7),
                AnalyticsPeriod.Month => DateTime.UtcNow.AddMonths(-1),
                AnalyticsPeriod.SixMonths => DateTime.UtcNow.AddMonths(-6),
                AnalyticsPeriod.AllTime => null,
                _ => DateTime.UtcNow.AddMonths(-1)
            };

            // 2. Фильтруем данные
            var milkHistory = dateFrom.HasValue
                ? cow.MilkHistory.Where(m => m.Date >= dateFrom.Value)
                : cow.MilkHistory;

            var foodHistory = dateFrom.HasValue
                ? cow.FoodHistory.Where(f => f.Date >= dateFrom.Value)
                : cow.FoodHistory;

            // 3. Считаем экономику (Доходы и Расходы)
            double milkPricePerLiter = _config.GetValue<double>("FarmConfig:MilkPricePerLiter");

            double totalMilkLiters = milkHistory.Sum(m => m.AmountInLiters);
            decimal totalMilkIncome = (decimal)(totalMilkLiters * milkPricePerLiter);

            decimal totalFoodCost = foodHistory.Sum(f => f.Cost);

            // 4. ROI (Коэффициент)
            double ratio = 0;
            if (totalFoodCost > 0)
            {
                ratio = (double)(totalMilkIncome / totalFoodCost);
            }
            else if (totalMilkIncome > 0)
            {
                ratio = 100.0; // Прибыль без затрат
            }

            // 5. Обновляем статус в БД
            double threshold = _config.GetValue<double>("FarmConfig:MilkToFoodRatioThreshold");
            bool flagForSlaughter = (ratio < threshold && totalFoodCost > 0);

            // Здесь не нужен cowRepository.Update(cow) и SaveChangesAsync(), 
            // т.к. этот код дублирует то, что уже есть в CowService.
            // Однако, в контексте ProductionService нам нужно обновить флаг.
            // Примечание: В идеале, CowService должен быть единственным, кто обновляет Cow.
            // Но сохраним текущую логику для минимальных изменений.
            _cowRepository.Update(cow);

            // --- Общие показатели ---
            double meatPricePerKg = _config.GetValue<double>("FarmConfig:MeatPricePerKg");
            double latestWeight = cow.WeightHistory.OrderByDescending(w => w.Date).FirstOrDefault()?.WeightInKg ?? 0;
            decimal estimatedMeatValue = (decimal)(latestWeight * meatPricePerKg);

            // Сохраняем
            await _unitOfWork.SaveChangesAsync();

            // Возвращаем данные ИМЕННО ЗА ЗАПРОШЕННЫЙ ПЕРИОД
            return new AnalyticsResponse
            {
                CowId = cowId,
                IsFlaggedForSlaughter = flagForSlaughter,
                MilkToFoodRatio = ratio,
                EstimatedMeatValue = estimatedMeatValue,
                TotalMilkIncome = totalMilkIncome, // Доход за период
                TotalUpkeepCost = totalFoodCost    // Расход за период
            };
        }
    }
}
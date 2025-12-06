using FarmManager.Application.Common.Interfaces;
using FarmManager.Application.Common.Models.Cows;
using FarmManager.Application.Common.Repositories;
using FarmManager.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace FarmManager.Application.Services
{
    public class CowService : ICowService
    {
        private readonly ICowRepository _cowRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public CowService(ICowRepository cowRepository, IUnitOfWork unitOfWork, IConfiguration config)
        {
            _cowRepository = cowRepository;
            _unitOfWork = unitOfWork;
            _config = config;
        }

        // --- ИЗМЕНЕНО: Принимаем userId и сохраняем его ---
        public async Task<CowResponse> CreateCowAsync(int userId, CreateCowRequest request)
        {
            var cow = new Cow
            {
                Name = request.Name,
                Breed = request.Breed,
                BirthDate = request.BirthDate,
                ApplicationUserId = userId // <--- СОХРАНЕНИЕ ID
            };
            await _cowRepository.AddAsync(cow);
            await _unitOfWork.SaveChangesAsync();
            return MapToCowResponse(cow);
        }

        // --- ИЗМЕНЕНО: Принимаем userId и проверяем его ---
        public async Task DeleteCowAsync(int id, int userId)
        {
            var cow = await _cowRepository.GetByIdAsync(id, userId);
            if (cow == null) throw new KeyNotFoundException("Корова не найдена.");
            _cowRepository.Delete(cow);
            await _unitOfWork.SaveChangesAsync();
        }

        // --- ИЗМЕНЕНО: Принимаем userId и передаем в репозиторий ---
        public async Task<IEnumerable<CowResponse>> GetAllCowsAsync(int userId, AnalyticsPeriod period = AnalyticsPeriod.AllTime)
        {
            var cows = await _cowRepository.GetAllAsync(userId); // <--- ПЕРЕДАЕМ ID

            DateTime? dateFrom = period switch
            {
                AnalyticsPeriod.Week => DateTime.UtcNow.AddDays(-7),
                AnalyticsPeriod.Month => DateTime.UtcNow.AddMonths(-1),
                AnalyticsPeriod.SixMonths => DateTime.UtcNow.AddMonths(-6),
                AnalyticsPeriod.AllTime => null,
                _ => null
            };

            return cows.Select(c => MapToCowResponse(c, dateFrom));
        }

        // --- ИЗМЕНЕНО: Принимаем userId и передаем в репозиторий ---
        public async Task<CowResponse?> GetCowByIdAsync(int id, int userId, AnalyticsPeriod period = AnalyticsPeriod.AllTime)
        {
            var cow = await _cowRepository.GetByIdWithHistoryAsync(id, userId); // <--- ПЕРЕДАЕМ ID
            if (cow == null) return null;

            DateTime? dateFrom = period switch
            {
                AnalyticsPeriod.Week => DateTime.UtcNow.AddDays(-7),
                AnalyticsPeriod.Month => DateTime.UtcNow.AddMonths(-1),
                AnalyticsPeriod.SixMonths => DateTime.UtcNow.AddMonths(-6),
                AnalyticsPeriod.AllTime => null,
                _ => null
            };

            return MapToCowResponse(cow, dateFrom);
        }

        // --- ИЗМЕНЕНО: Принимаем userId и проверяем его ---
        public async Task UpdateCowAsync(int id, int userId, UpdateCowRequest request)
        {
            var cow = await _cowRepository.GetByIdAsync(id, userId);
            if (cow == null) throw new KeyNotFoundException("Корова не найдена.");

            cow.Name = request.Name;
            cow.Breed = request.Breed;
            cow.BirthDate = request.BirthDate;

            _cowRepository.Update(cow);
            await _unitOfWork.SaveChangesAsync();
        }

        private CowResponse MapToCowResponse(Cow cow, DateTime? filterDateFrom = null)
        {
            decimal milkPrice = _config.GetValue<decimal>("FarmConfig:MilkPricePerLiter");
            double threshold = _config.GetValue<double>("FarmConfig:MilkToFoodRatioThreshold");

            var milks = cow.MilkHistory.AsEnumerable();
            var foods = cow.FoodHistory.AsEnumerable();

            if (filterDateFrom.HasValue)
            {
                milks = milks.Where(m => m.Date >= filterDateFrom.Value);
                foods = foods.Where(f => f.Date >= filterDateFrom.Value);
            }

            double totalMilkLiters = milks.Sum(m => m.AmountInLiters);
            decimal totalMilkIncome = (decimal)totalMilkLiters * milkPrice;
            decimal totalFoodCost = foods.Sum(f => f.Cost);

            double ratio = 0;
            if (totalFoodCost > 0) ratio = (double)(totalMilkIncome / totalFoodCost);
            else if (totalMilkIncome > 0) ratio = 100.0;

            bool isFlaggedDynamic = (ratio < threshold && totalFoodCost > 0);

            var weights = cow.WeightHistory.AsEnumerable();
            if (filterDateFrom.HasValue)
            {
                weights = weights.Where(w => w.Date >= filterDateFrom.Value);
            }

            return new CowResponse
            {
                Id = cow.Id,
                Name = cow.Name,
                Breed = cow.Breed,
                BirthDate = cow.BirthDate,

                IsFlaggedForSlaughter = isFlaggedDynamic,
                ProfitRatio = ratio,
                TotalIncome = totalMilkIncome,

                WeightHistory = weights.Select(w => new CowResponse.WeightRecordDto
                {
                    Id = w.Id,
                    Date = w.Date,
                    WeightInKg = w.WeightInKg,
                    PhotoUrl = w.PhotoUrl
                }).OrderByDescending(w => w.Date),

                MilkHistory = milks.Select(m => new CowResponse.MilkYieldDto
                {
                    Id = m.Id,
                    Date = m.Date,
                    AmountInLiters = m.AmountInLiters,
                    Value = (decimal)m.AmountInLiters * milkPrice
                }).OrderByDescending(m => m.Date),

                FoodHistory = foods.Select(f => new CowResponse.FoodConsumptionDto
                {
                    Id = f.Id,
                    Date = f.Date,
                    AmountInKg = f.AmountInKg,
                    Cost = f.Cost
                }).OrderByDescending(f => f.Date)
            };
        }
    }
}
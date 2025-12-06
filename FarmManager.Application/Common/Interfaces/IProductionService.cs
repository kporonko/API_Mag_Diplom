using FarmManager.Application.Common.Models.Cows;
using FarmManager.Application.Common.Models.Production;
using FarmManager.Application.Services;
using FarmManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Interfaces
{
    public interface IProductionService
    {
        Task<CowResponse.MilkYieldDto> AddMilkYieldAsync(int userId, AddMilkRequest request);
        Task<CowResponse.FoodConsumptionDto> AddFoodConsumptionAsync(int userId, AddFoodRequest request);
        Task<CowResponse.WeightRecordDto> CalculateWeightAsync(int userId, CalculateWeightRequest request);

        Task<AnalyticsResponse> RunAnalyticsAsync(int cowId, int userId, AnalyticsPeriod period = AnalyticsPeriod.Month);
    }
}

using FarmManager.Application.Common.Models.Cows;
using FarmManager.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Interfaces
{
    public interface ICowService
    {
        Task<IEnumerable<CowResponse>> GetAllCowsAsync(int userId, AnalyticsPeriod period = AnalyticsPeriod.AllTime);

        Task<CowResponse?> GetCowByIdAsync(int id, int userId, AnalyticsPeriod period = AnalyticsPeriod.AllTime);
        Task<CowResponse> CreateCowAsync(int userId, CreateCowRequest request);
        Task UpdateCowAsync(int id, int userId, UpdateCowRequest request);
        Task DeleteCowAsync(int id, int userId);
    }
}

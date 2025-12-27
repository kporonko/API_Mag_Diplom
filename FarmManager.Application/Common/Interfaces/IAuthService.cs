using FarmManager.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Interfaces
{
    public interface IAuthService
    {
        // Либо вернет AuthResponse, либо список ошибок (e.g., "User already exists")
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserProfileDto> GetUserProfileAsync(int userId);
        Task<UserProfileDto> UpdateUserProfileAsync(int userId, UpdateProfileRequest request);
    }
}

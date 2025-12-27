using FarmManager.Application.Common.Interfaces;
using FarmManager.Application.Common.Models;
using FarmManager.Application.Common.Repositories;
using FarmManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<UserProfileDto> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            return new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Language = user.Language,
                DefaultMilkPrice = user.DefaultMilkPrice,
                DefaultFoodPrice = user.DefaultFoodPrice,
                DefaultMeatPrice = user.DefaultMeatPrice
            };
        }

        public async Task<UserProfileDto> UpdateUserProfileAsync(int userId, UpdateProfileRequest request)
        {
            // 1. Получаем пользователя из БД (EF Core отслеживает изменения этого объекта)
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            // 2. Обновляем поля
            user.FullName = request.FullName;
            user.DefaultMilkPrice = request.DefaultMilkPrice;
            user.DefaultFoodPrice = request.DefaultFoodPrice;
            user.DefaultMeatPrice = request.DefaultMeatPrice;
            user.Language = request.Language;

            // 3. Сохраняем изменения
            // (В EF Core вызывать Update не обязательно, если объект получен из контекста, 
            // но SaveChanges нужен обязательно)
            await _unitOfWork.SaveChangesAsync();

            // 4. Возвращаем обновленные данные
            return new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Language = user.Language,
                DefaultMilkPrice = user.DefaultMilkPrice,
                DefaultFoodPrice = user.DefaultFoodPrice,
                DefaultMeatPrice = user.DefaultMeatPrice
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            // 1. Найти юзера
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            // 2. Проверить, что он есть и пароль совпадает
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new AuthenticationException("Неверный логин или пароль.");
            }

            // 3. Сгенерировать токен
            var token = _jwtTokenGenerator.GenerateToken(user);

            // 4. Вернуть ответ
            return new AuthResponse
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Token = token
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // 1. Проверить, не занят ли username
            if (await _userRepository.GetByUsernameAsync(request.Username) != null)
            {
                throw new InvalidOperationException("Пользователь с таким именем уже существует.");
            }

            var user = new ApplicationUser
            {
                Username = request.Username,
                FullName = request.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthResponse
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Token = token
            };
        }
    }
}

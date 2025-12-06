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

            // 2. Создать юзера
            var user = new ApplicationUser
            {
                Username = request.Username,
                FullName = request.FullName,
                // 3. ЗАХЕШИРОВАТЬ ПАРОЛЬ
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            // 4. Сохранить в БД
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(); // <-- Коммитим транзакцию

            // 5. Сгенерировать токен
            var token = _jwtTokenGenerator.GenerateToken(user);

            // 6. Вернуть ответ
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

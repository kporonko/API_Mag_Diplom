using FarmManager.Application.Common.Repositories;
using FarmManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmManager.Infrastructure.Persistence.Repositories
{
    public class CowRepository : ICowRepository
    {
        private readonly ApplicationDbContext _context;

        public CowRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Cow cow)
        {
            await _context.Cows.AddAsync(cow);
        }

        public void Delete(Cow cow)
        {
            _context.Cows.Remove(cow);
        }

        // --- ОБНОВЛЕНО: ФИЛЬТРАЦИЯ по UserId ---
        public async Task<IEnumerable<Cow>> GetAllAsync(int userId)
        {
            return await _context.Cows
                .Where(c => c.ApplicationUserId == userId) // <--- Фильтр по ID
                .Include(c => c.WeightHistory)
                .Include(c => c.MilkHistory)
                .Include(c => c.FoodHistory)
                .ToListAsync();
        }

        public async Task<Cow?> GetByIdAsync(int id, int userId)
        {
            // Находим корову только если она принадлежит этому пользователю
            return await _context.Cows.FirstOrDefaultAsync(c => c.Id == id && c.ApplicationUserId == userId);
        }

        public async Task<Cow?> GetByIdWithHistoryAsync(int id, int userId)
        {
            return await _context.Cows
                .AsNoTracking() // Опционально: для аналитики полезно (быстрее), если не собираешься менять саму сущность User
                .Where(c => c.ApplicationUserId == userId && c.Id == id)
                .Include(c => c.WeightHistory)
                .Include(c => c.MilkHistory)
                .Include(c => c.FoodHistory)
                .Include(c => c.ApplicationUser) // <--- ВОТ ТУТ МЫ ПОДГРУЖАЕМ ЮЗЕРА
                .FirstOrDefaultAsync();
        }

        public void Update(Cow cow)
        {
            _context.Cows.Update(cow);
        }
    }
}
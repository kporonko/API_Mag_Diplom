using FarmManager.Domain.Entities;
using FarmManager.Domain.Entities;
using FarmManager.Infrastructure.Persistence; // <-- нужен DbContext
using Microsoft.EntityFrameworkCore;
using FarmManager.Application.Common.Repositories;

namespace FarmManager.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        // Репозиторий зависит от ApplicationDbContext
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ApplicationUser user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<ApplicationUser?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<ApplicationUser?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}

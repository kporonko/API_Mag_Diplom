using FarmManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(int id);
        Task<ApplicationUser?> GetByUsernameAsync(string username);
        Task AddAsync(ApplicationUser user);
        // Update/Delete по необходимости
    }
}

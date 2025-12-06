using FarmManager.Application.Common.Repositories;
using FarmManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Infrastructure.Persistence.Repositories
{
    public class MilkYieldRepository : IMilkYieldRepository
    {
        private readonly ApplicationDbContext _context;

        public MilkYieldRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MilkYield record)
        {
            await _context.MilkYields.AddAsync(record);
        }
    }
}

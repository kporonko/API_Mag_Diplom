using FarmManager.Application.Common.Repositories;
using FarmManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Infrastructure.Persistence.Repositories
{
    public class WeightRecordRepository : IWeightRecordRepository
    {
        private readonly ApplicationDbContext _context;

        public WeightRecordRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(WeightRecord record)
        {
            await _context.WeightRecords.AddAsync(record);
        }
    }
}

using FarmManager.Application.Common.Repositories;
using FarmManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Infrastructure.Persistence.Repositories
{
    public class FoodConsumptionRepository : IFoodConsumptionRepository
    {
        private readonly ApplicationDbContext _context;

        public FoodConsumptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FoodConsumption record)
        {
            await _context.FoodConsumptions.AddAsync(record);
        }
    }
}

using FarmManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cow> Cows { get; set; } = null!;
        public DbSet<WeightRecord> WeightRecords { get; set; } = null!;
        public DbSet<MilkYield> MilkYields { get; set; } = null!;
        public DbSet<FoodConsumption> FoodConsumptions { get; set; } = null!;
        public DbSet<ApplicationUser> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }
    }
}
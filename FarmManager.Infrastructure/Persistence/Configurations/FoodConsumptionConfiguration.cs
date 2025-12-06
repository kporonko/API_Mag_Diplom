using FarmManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Infrastructure.Persistence.Configurations
{
    public class FoodConsumptionConfiguration : IEntityTypeConfiguration<FoodConsumption>
    {
        public void Configure(EntityTypeBuilder<FoodConsumption> builder)
        {
            builder.HasKey(f => f.Id);

            // Настраиваем точность для денег (decimal)
            builder.Property(f => f.Cost)
                .HasPrecision(18, 2); // Точность для стоимости

            builder.Property(f => f.AmountInKg)
                .HasPrecision(18, 2); // Точность для веса
        }
    }
}

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
    public class WeightRecordConfiguration : IEntityTypeConfiguration<WeightRecord>
    {
        public void Configure(EntityTypeBuilder<WeightRecord> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.WeightInKg)
                .HasPrecision(18, 2); // Точность для веса (e.g., 123.45 кг)

            builder.Property(w => w.PhotoUrl)
                .IsRequired();
        }
    }
}

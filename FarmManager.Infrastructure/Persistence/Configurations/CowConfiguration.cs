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
    public class CowConfiguration : IEntityTypeConfiguration<Cow>
    {
        public void Configure(EntityTypeBuilder<Cow> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(c => c.Breed)
                .HasMaxLength(100);

            // --- ОБНОВЛЕНО: Конфигурация связи с пользователем ---
            builder.HasOne(c => c.ApplicationUser)
                            .WithMany()
                            .HasForeignKey(c => c.ApplicationUserId) // <-- Используем ваше явно определенное свойство
                            .HasPrincipalKey(u => u.Id)             // <-- (Опционально, но помогает) Указываем, что это связывается с Id пользователя
                            .IsRequired()
                            .OnDelete(DeleteBehavior.Cascade);
            // ---------------------------------------------------

            // Настройка связей "Один-ко-многим"
            builder.HasMany(c => c.WeightHistory)
                .WithOne(w => w.Cow)
                .HasForeignKey(w => w.CowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.MilkHistory)
                .WithOne(m => m.Cow)
                .HasForeignKey(m => m.CowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.FoodHistory)
                .WithOne(f => f.Cow)
                .HasForeignKey(f => f.CowId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
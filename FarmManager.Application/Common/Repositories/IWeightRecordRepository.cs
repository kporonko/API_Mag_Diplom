using FarmManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Repositories
{
    public interface IWeightRecordRepository
    {
        Task AddAsync(WeightRecord record);
        // (Можем добавить Get, Delete и т.д. по необходимости)
    }
}

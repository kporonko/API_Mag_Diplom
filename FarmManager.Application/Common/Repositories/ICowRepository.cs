using FarmManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Repositories
{
    public interface ICowRepository
    {
        // --- ОБНОВЛЕНО: Добавлен userId для фильтрации/проверки ---
        Task<Cow?> GetByIdAsync(int id, int userId);
        Task<IEnumerable<Cow>> GetAllAsync(int userId);

        // --- ОБНОВЛЕНО: Добавлен userId для фильтрации ---
        Task<Cow?> GetByIdWithHistoryAsync(int id, int userId);

        // Методы без userId (для добавления/обновления/удаления сущности)
        Task AddAsync(Cow cow);
        void Update(Cow cow);
        void Delete(Cow cow);
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Interfaces
{
    public interface IFileStorage
    {
        // Возвращает URL (или путь) к сохраненному файлу
        Task<string> SaveFileAsync(IFormFile file, string subfolder);
    }
}

using FarmManager.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Services
{
    public class LocalStorage : IFileStorage
    {
        private readonly string _uploadFolder;
        private readonly IHostEnvironment _hostEnvironment;

        public LocalStorage(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            // wwwroot - это папка, которую ASP.NET Core может "отдавать" наружу
            _uploadFolder = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "uploads");
            Directory.CreateDirectory(_uploadFolder);
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subfolder)
        {
            var folderPath = Path.Combine(_uploadFolder, subfolder);
            Directory.CreateDirectory(folderPath);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(folderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Возвращаем ПУТЬ, по которому файл будет доступен из веба
            // (e.g., /uploads/cow_weights/GUID_photo.jpg)
            return Path.Combine("/uploads", subfolder, uniqueFileName).Replace("\\", "/");
        }
    }
}

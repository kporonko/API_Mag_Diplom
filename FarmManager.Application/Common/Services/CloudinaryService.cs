using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FarmManager.Application.Common.Interfaces;
using FarmManager.Application.Common.Models.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Services
{
    public class CloudinaryService : IFileStorage
    {
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _configuration;

        public CloudinaryService(IConfiguration configuration)
        {
            _configuration = configuration;

            var settings = new CloudinarySettings();
            _configuration.Bind(CloudinarySettings.SectionName, settings);

            if (string.IsNullOrEmpty(settings.CloudName) ||
                string.IsNullOrEmpty(settings.ApiKey) ||
                string.IsNullOrEmpty(settings.ApiSecret))
            {
                throw new ArgumentException("Cloudinary settings (CloudName, ApiKey, ApiSecret) must be set in appsettings.json");
            }

            var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true; // Использовать SecureUrl, как ты и указал
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subfolder)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File not provided");
            }

            // Уникальный PublicId, чтобы не было перезаписей
            var publicId = $"farm_manager/{subfolder}/{Guid.NewGuid()}_{Path.GetFileNameWithoutExtension(file.FileName)}";

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = publicId,
                Folder = $"farm_manager/{subfolder}"
                // Мы не указываем трансформации при загрузке,
                // чтобы сохранить оригинал. Мы их применим при *показе*.
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString();
            }

            throw new Exception($"Cloudinary upload failed: {uploadResult.Error?.Message}");
        }
    }
}

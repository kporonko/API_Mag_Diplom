using FarmManager.Application.Common.Interfaces;
using FarmManager.Application.Common.Models.Cows;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration; // Для IConfiguration
using System.Net.Http.Json; // Для ReadFromJsonAsync

// 1. ИЗМЕНЯЕМ NAMESPACE
namespace FarmManager.Application.Common.Services
{
    public class FlaskMlClient : IFlaskMlClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _flaskApiUrl;

        // 1. ИЗМЕНЯЕМ КОНСТРУКТОР
        // Убираем IHttpClientFactory, внедряем HttpClient НАПРЯМУЮ
        public FlaskMlClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient; // <-- 2. Просто присваиваем
            _flaskApiUrl = configuration.GetValue<string>("FarmConfig:FlaskApiUrl")
                ?? throw new InvalidOperationException("FlaskApiUrl не настроен в appsettings.json");
        }

        public async Task<double> GetWeightFromPhotoAsync(IFormFile photo)
        {
            // Используем using, чтобы ресурсы освободились
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(photo.OpenReadStream());

            streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
            {
                Name = "file", // Имя должно совпадать с тем, что ожидает Flask
                FileName = photo.FileName
            };

            content.Add(streamContent);

            content.Add(new StringContent("weight"), "type");

            var response = await _httpClient.PostAsync(_flaskApiUrl, content);
            response.EnsureSuccessStatusCode(); // Выкинет ошибку, если Flask не ответил 200 OK

            var flaskResponse = await response.Content.ReadFromJsonAsync<FlaskWeightResponse>();

            if (flaskResponse == null || flaskResponse.EstimatedWeightKg <= 0)
            {
                throw new InvalidOperationException("ML сервис вернул неверный вес.");
            }

            return flaskResponse.EstimatedWeightKg;
        }
    }
}
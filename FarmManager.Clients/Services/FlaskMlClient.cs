using FarmManager.Application.Common.Interfaces;
using FarmManager.Application.Common.Interfaces.Infrastructure;
using FarmManager.Application.Common.Models.Production;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration; // Для IConfiguration
using System.Net.Http.Json; // Для ReadFromJsonAsync

// 1. ИЗМЕНЯЕМ NAMESPACE
namespace FarmManager.Clients.Services
{
    public class FlaskMlClient : IFlaskMlClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _flaskApiUrl;

        public FlaskMlClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("FlaskClient");
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

            var response = await _httpClient.PostAsync(_flaskApiUrl, content);
            response.EnsureSuccessStatusCode(); // Выкинет ошибку, если Flask не ответил 200 OK

            var flaskResponse = await response.Content.ReadFromJsonAsync<FlaskWeightResponse>();

            if (flaskResponse == null || flaskResponse.Weight <= 0)
            {
                throw new InvalidOperationException("ML сервис вернул неверный вес.");
            }

            return flaskResponse.Weight;
        }
    }
}
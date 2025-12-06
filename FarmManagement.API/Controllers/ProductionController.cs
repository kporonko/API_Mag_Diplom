using FarmManager.Application.Common.Interfaces;
using FarmManager.Application.Common.Models.Cows;
using FarmManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductionController : ControllerBase
    {
        private readonly IProductionService _productionService;

        public ProductionController(IProductionService productionService)
        {
            _productionService = productionService;
        }

        // --- ВСПОМОГАТЕЛЬНЫЙ МЕТОД: Получение ID пользователя из токена ---
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User ID claim not found or invalid.");
        }
        // -------------------------------------------------------------------

        [HttpPost("milk")]
        public async Task<IActionResult> AddMilk([FromBody] AddMilkRequest request)
        {
            var userId = GetUserId(); // <--- ПОЛУЧАЕМ ID
            try
            {
                var result = await _productionService.AddMilkYieldAsync(userId, request); // <--- ПЕРЕДАЕМ ID
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("food")]
        public async Task<IActionResult> AddFood([FromBody] AddFoodRequest request)
        {
            var userId = GetUserId(); // <--- ПОЛУЧАЕМ ID
            try
            {
                var result = await _productionService.AddFoodConsumptionAsync(userId, request); // <--- ПЕРЕДАЕМ ID
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("weight")]
        public async Task<IActionResult> CalculateWeight([FromForm] CalculateWeightRequest request)
        {
            if (request.Photo == null || request.Photo.Length == 0)
                return BadRequest(new { message = "Фото не предоставлено." });

            var userId = GetUserId(); // <--- ПОЛУЧАЕМ ID
            try
            {
                var result = await _productionService.CalculateWeightAsync(userId, request); // <--- ПЕРЕДАЕМ ID
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Внутренняя ошибка: {ex.Message}" });
            }
        }

        // --- ИСПРАВЛЕНО: Добавлен userId ---
        [HttpGet("analytics/{cowId}")]
        public async Task<IActionResult> RunAnalytics(int cowId, [FromQuery] AnalyticsPeriod period = AnalyticsPeriod.Month)
        {
            var userId = GetUserId(); // <--- ПОЛУЧАЕМ ID
            try
            {
                var result = await _productionService.RunAnalyticsAsync(cowId, userId, period); // <--- ПЕРЕДАЕМ ID
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
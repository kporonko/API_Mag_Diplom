using FarmManager.Application.Common.Interfaces;
using FarmManager.Application.Common.Models.Cows;
using FarmManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // Для работы с Claims

namespace FarmManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CowsController : ControllerBase
    {
        private readonly ICowService _cowService;

        public CowsController(ICowService cowService)
        {
            _cowService = cowService;
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

        [HttpGet]
        public async Task<IActionResult> GetAllCows([FromQuery] AnalyticsPeriod period = AnalyticsPeriod.AllTime)
        {
            var userId = GetUserId(); // <--- ПОЛУЧАЕМ ID
            var cows = await _cowService.GetAllCowsAsync(userId, period); // <--- ПЕРЕДАЕМ ID
            return Ok(cows);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCowById(int id, [FromQuery] AnalyticsPeriod period = AnalyticsPeriod.AllTime)
        {
            var userId = GetUserId(); // <--- ПОЛУЧАЕМ ID
            var cow = await _cowService.GetCowByIdAsync(id, userId, period); // <--- ПЕРЕДАЕМ ID
            if (cow == null) return NotFound();
            return Ok(cow);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCow([FromBody] CreateCowRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = GetUserId(); // <--- ПОЛУЧАЕМ ID
            var createdCow = await _cowService.CreateCowAsync(userId, request); // <--- ПЕРЕДАЕМ ID
            return CreatedAtAction(nameof(GetCowById), new { id = createdCow.Id }, createdCow);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCow(int id, [FromBody] UpdateCowRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var userId = GetUserId(); // <--- ПОЛУЧАЕМ ID
                await _cowService.UpdateCowAsync(id, userId, request); // <--- ПЕРЕДАЕМ ID
                return NoContent();
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCow(int id)
        {
            try
            {
                var userId = GetUserId(); // <--- ПОЛУЧАЕМ ID
                await _cowService.DeleteCowAsync(id, userId); // <--- ПЕРЕДАЕМ ID
                return NoContent();
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }
    }
}
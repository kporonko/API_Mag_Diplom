using FarmManager.Application.Common.Interfaces;
using FarmManager.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManagement.API.Controllers
{
    [Authorize] // Весь контроллер требует авторизации
    [ApiController]
    [Route("api/[controller]")] // Будет доступен по адресу /api/Profile
    public class ProfileController : ControllerBase
    {
        private readonly IAuthService _authService;

        public ProfileController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET: api/Profile
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = GetUserId();
                var userProfile = await _authService.GetUserProfileAsync(userId);
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Profile
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var userId = GetUserId();
                var updatedProfile = await _authService.UpdateUserProfileAsync(userId, request);
                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Вспомогательный метод
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Invalid User ID in token.");
        }
    }
}
using FarmManager.Application.Common.Interfaces;
using FarmManager.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace FarmManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var authResponse = await _authService.RegisterAsync(request);
                return Ok(authResponse);
            }
            catch (InvalidOperationException ex) // e.g., "User already exists"
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var authResponse = await _authService.LoginAsync(request);
                return Ok(authResponse);
            }
            catch (AuthenticationException ex) // "Invalid username or password"
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
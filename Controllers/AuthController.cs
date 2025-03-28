using be.Dtos.Auth;
using be.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _authService.LoginAsync(loginDTO);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var (user, emailToken) = await _authService.RegisterAsync(registerDTO);
                return Ok(new { User = user, EmailConfirmationToken = emailToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailDTO confirmEmailDTO)
        {
            try
            {
                await _authService.ConfirmEmailAsync(confirmEmailDTO);
                return Ok("Email confirmed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Errors = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            await _authService.LogoutAsync();
            return Ok("Logged out successfully.");
        }
    }
}
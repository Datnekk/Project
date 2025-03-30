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
        private readonly IOtpService _otpService;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, IOtpService otpService)
        {
            _authService = authService;
            _logger = logger;
            _otpService = otpService;
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
                return Ok(new { User = user, EmailConfirmationToken = emailToken, Message = "OTP sent to email." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtpAsync([FromBody] ConfirmOtpDTO confirmOtpDTO)
        {
            try
            {
                if (confirmOtpDTO == null)
                {
                    return BadRequest("Invalid request.");
                }
                if (_otpService == null)
                {
                    Console.WriteLine("Error: _otpService is NULL in AuthController!");
                    throw new ArgumentNullException(nameof(_otpService));
                }

                if (confirmOtpDTO == null)
                {
                    Console.WriteLine("Error: confirmOtpDTO is NULL in VerifyOtpAsync!");
                    throw new ArgumentNullException(nameof(confirmOtpDTO));
                }

                var isVerified = await _otpService.VerifyOtpAsync(confirmOtpDTO ?? throw new ArgumentNullException(nameof(confirmOtpDTO)));
                if (!isVerified) return BadRequest("Invalid or expired OTP.");

                return Ok("OTP verified successfully.");
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in VerifyOtpAsync: {ex}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
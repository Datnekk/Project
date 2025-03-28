using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using be.Dtos.Auth;
using be.Models;
using be.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace be.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IUserContext _userContext;
        private readonly SignInManager<User> _signInManager;

        public AuthController(ILogger<AuthController> logger,IMapper mapper, UserManager<User> userManager, ITokenService tokenService, IUserContext userContext, SignInManager<User> signInManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _tokenService = tokenService;
            _userContext = userContext;
            _signInManager = signInManager;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO loginDTO){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == loginDTO.Email);

            if(user == null) return Unauthorized("Invalid Email!");

            var res = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

            if(!res.Succeeded) return Unauthorized("Email/Password Not Found");

            var accessToken = await _tokenService.CreateJWTTokenAsync(user);
            
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

            var response = _mapper.Map<AuthResponseDTO>(user);
            response.AccessToken = accessToken;
            response.RefreshToken = refreshToken;

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = _mapper.Map<User>(registerDTO);
                user.NormalizedUserName = _userManager.NormalizeName(registerDTO.UserName);
                user.NormalizedEmail = _userManager.NormalizeEmail(registerDTO.Email);

                var createdUser = await _userManager.CreateAsync(user, registerDTO.Password);
                if (!createdUser.Succeeded)
                {
                    return StatusCode(500, createdUser.Errors);
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return StatusCode(500, roleResult.Errors);
                }

                var emailToken = await _tokenService.GenerateEmailConfirmationTokenAsync(user.Id);

                //Implement send confirmation email here

                var accessToken = await _tokenService.CreateJWTTokenAsync(user);
                var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

                var response = _mapper.Map<AuthResponseDTO>(user);
                response.AccessToken = accessToken;
                response.RefreshToken = refreshToken;

                return Ok(new { User = response, EmailConfirmationToken = emailToken });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailDTO confirmEmailDTO)
        {
            var (succeeded, errors) = await _tokenService.ConfirmEmailAsync(confirmEmailDTO.UserId, confirmEmailDTO.Token);
            if (!succeeded)
            {
                return BadRequest(new { Errors = errors });
            }
            return Ok("Email confirmed successfully.");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequestDTO requestDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(requestDTO.AccessToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false, 
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _tokenService.GetIssuer(),
                    ValidAudience = _tokenService.GetAudience(),
                    IssuerSigningKey = _tokenService.GetKey()
                }, out var validatedToken);

                var userId = await _userContext.GetCurrentUserIdAsync();

                // Verify the refresh token
                var (isValid, error) = await _tokenService.VerifyRefreshTokenAsync(userId, requestDTO.RefreshToken);
                if (!isValid)
                {
                    return Unauthorized(error);
                }

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return Unauthorized("User not found.");
                }

                var newAccessToken = await _tokenService.CreateJWTTokenAsync(user);
                var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

                await _tokenService.RemoveRefreshTokenAsync(user.Id);

                var response = _mapper.Map<AuthResponseDTO>(user);
                response.AccessToken = newAccessToken;
                response.RefreshToken = newRefreshToken;

                return Ok(response);
            }
            catch (SecurityTokenException)
            {
                return Unauthorized("Invalid token.");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            var userId = await _userContext.GetCurrentUserIdAsync();
            Console.WriteLine($"User ID: {userId}");

            await _tokenService.RemoveRefreshTokenAsync(userId);

            await _signInManager.SignOutAsync();

            return Ok("Logged out successfully.");
        }

        [HttpGet("external-login")]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl }, protocol: Request.Scheme);
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl, string remoteError)
        {
            if (remoteError != null)
            {
                return BadRequest(new { message = $"Error from external provider: {remoteError}" });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest(new { message = "Error loading external login information." });
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            string accessToken = string.Empty; 
            string refreshToken = string.Empty;
            AuthResponseDTO response = new();

            if (result.Succeeded)
            {
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                accessToken = await _tokenService.CreateJWTTokenAsync(user);
                refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

                response = _mapper.Map<AuthResponseDTO>(user);
                response.AccessToken = accessToken;
                response.RefreshToken = refreshToken;

                return Ok(new { user = response, returnUrl });
            }

            // If the user doesn't have an account, create one
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            var picture = info.Principal.FindFirstValue("picture");

            var newUser = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Image = picture
            };

            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
            {
                return BadRequest(new { errors = createResult.Errors.Select(e => e.Description) });
            }

            var addLoginResult = await _userManager.AddLoginAsync(newUser, info);
            if (!addLoginResult.Succeeded)
            {
                return BadRequest(new { errors = addLoginResult.Errors.Select(e => e.Description) });
            }

            await _signInManager.SignInAsync(newUser, isPersistent: true);

            accessToken = await _tokenService.CreateJWTTokenAsync(newUser);
            refreshToken = await _tokenService.GenerateRefreshTokenAsync(newUser.Id);

            response = _mapper.Map<AuthResponseDTO>(newUser);
            response.AccessToken = accessToken;
            response.RefreshToken = refreshToken;

            return Ok(new { user = response, returnUrl });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { message = "User not found." });
            }

            var response = _mapper.Map<AuthResponseDTO>(user);
            return Ok(response);
        }
    }
}
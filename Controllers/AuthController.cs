using System.IdentityModel.Tokens.Jwt;
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
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;

        public AuthController(IMapper mapper, UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO loginDTO){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDTO.UserName);

            if(user == null) return Unauthorized("Invalid UserName!");

            var res = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

            if(!res.Succeeded) return Unauthorized("UserName/Password Not Found");

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

                var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Invalid token.");
                }

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

                // Generate new tokens
                var newAccessToken = await _tokenService.CreateJWTTokenAsync(user);
                var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

                // Remove the old refresh token
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
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            await _tokenService.RemoveRefreshTokenAsync(userId);
            await _signInManager.SignOutAsync();

            return Ok("Logged out successfully.");
        }
    }
}
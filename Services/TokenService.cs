using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using be.Data;
using be.Models;
using be.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace be.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public TokenService(IConfiguration configuration, UserManager<User> userManager, ApplicationDbContext context)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
        }
        public async Task<string> CreateJWTTokenAsync(User user)
        {
            var jwtKey = _configuration["JWT:Key"];
            var issuer = _configuration["JWT:Issuer"];
            var audience = _configuration["JWT:Audience"];
            var expiryDays = int.TryParse(_configuration["JWT:ExpiryDays"], out var days) ? days : 1;

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT configuration values are missing.");
            }

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()), 
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Name, user.Name ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
            };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(expiryDays),
                SigningCredentials = creds,
                Issuer = issuer,
                Audience = audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"),
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30), 
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken.Token;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<(bool Succeeded, string[] Errors)> ConfirmEmailAsync(int userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return (false, Array.Empty<string>());
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return (result.Succeeded, result.Succeeded ? [] : result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<(bool Succeeded, string Error)> VerifyRefreshTokenAsync(int userId, string token)
        {
            var refreshToken = await _context.RefreshTokens
                                     .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Token == token);

            if (refreshToken == null)
            {
                return (false, "Refresh token not found.");
            }

            if (refreshToken.IsRevoked)
            {
                return (false, "Refresh token has been revoked.");
            }

            if (refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                return (false, "Refresh token has expired.");
            }

            return (true, string.Empty);
        }

        public async Task RemoveRefreshTokenAsync(int userId)
        {
            var refreshToken = await _context.RefreshTokens
                                     .FirstOrDefaultAsync(rt => rt.UserId == userId && !rt.IsRevoked);
            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }

        public string GetIssuer() => _configuration["JWT:Issuer"] ?? string.Empty;
        public string GetAudience() => _configuration["JWT:Audience"] ?? string.Empty;
        public SymmetricSecurityKey GetKey() => _key;
    }
}
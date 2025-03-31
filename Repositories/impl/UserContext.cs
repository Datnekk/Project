
using System.Security.Claims;
using AutoMapper;
using be.Dtos.Users;
using be.Models;
using Microsoft.AspNetCore.Identity;

namespace be.Repositories.impl;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UserContext(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, IMapper mapper)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<UserReadDTO?> GetCurrentUserAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");

        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            return null; 
        }

        var user = await _userManager.GetUserAsync(httpContext.User) ?? throw new UnauthorizedAccessException("No authenticated user found.");

        if (user == null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        var userDto = _mapper.Map<UserReadDTO>(user);

        userDto.Role = roles;

        return userDto;
    }

    public int GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        
        if (user == null || !user.Identity.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID.");
        }

        return userId;
    }

    public async Task<int> GetCurrentUserIdAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null || !user.Identity.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }    

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID.");
        }

        var dbUser = await _userManager.FindByIdAsync(userId.ToString());

        if (dbUser == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }
        
        return userId;
    }
}
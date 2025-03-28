using AutoMapper;
using be.Dtos.Auth;
using be.Models;
using be.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace be.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AuthService(UserManager<User> userManager, ITokenService tokenService, IMapper mapper, SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _signInManager = signInManager;
    }

    public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO)
    {
        var user = await _userManager.FindByEmailAsync(loginDTO.Email!);

        if(user is null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password!)){
            return new AuthResponseDTO{
                IsAuthSuccessful = false,
                ErrorMessage = "Invalid Authentication"
            };
        }
       var tokenDTO = await _tokenService.CreateJWTTokenAsync(user, populateExp: true);

       _tokenService.SetTokenCookie(tokenDTO, _httpContextAccessor.HttpContext);

       return new AuthResponseDTO{
            IsAuthSuccessful = true,
       };
    }

    public async Task<(AuthResponseDTO User, string EmailConfirmationToken)> RegisterAsync(RegisterDTO registerDTO)
    {
        var user = _mapper.Map<User>(registerDTO);

        user.NormalizedUserName = _userManager.NormalizeName(registerDTO.UserName);
        
        user.NormalizedEmail = _userManager.NormalizeEmail(registerDTO.Email);

        var createdUser = await _userManager.CreateAsync(user, registerDTO.Password);

        if (!createdUser.Succeeded)
        {
            throw new Exception(string.Join(", ", createdUser.Errors.Select(e => e.Description)));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "User");
        
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            throw new Exception(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
        }

        var emailToken = await _tokenService.GenerateEmailConfirmationTokenAsync(user.Id);
        // TODO: Implement sending the confirmation email here

        var response = _mapper.Map<AuthResponseDTO>(user);

        return (response, emailToken);
    }

    public async Task<bool> ConfirmEmailAsync(ConfirmEmailDTO confirmEmailDTO)
    {
        var (succeeded, errors) = await _tokenService.ConfirmEmailAsync(confirmEmailDTO.UserId, confirmEmailDTO.Token);
        if (!succeeded)
        {
            throw new Exception(string.Join(", ", errors));
        }
        return true;
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
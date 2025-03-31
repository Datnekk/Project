using AutoMapper;
using be.Dtos.Auth;
using be.Models;
using be.Repositories;
using Microsoft.AspNetCore.Identity;

namespace be.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;
    public AuthService(UserManager<User> userManager, ITokenService tokenService, IMapper mapper, SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor, IOtpService otpService, IEmailService emailService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _signInManager = signInManager;
        _otpService = otpService;
        _emailService = emailService;
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

        user.SecurityStamp = Guid.NewGuid().ToString();

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
        var otp = new Random().Next(100000, 999999).ToString();
        await _otpService.SaveOtpAsync(registerDTO.Email, otp);
        await _emailService.SendEmailAsync(registerDTO.Email, "OTP Verification", $"Your OTP is: {otp}");

        var response = _mapper.Map<AuthResponseDTO>(user);

        return (response, emailToken);
    }

    public async Task<bool> VerifyOtpAsync(ConfirmOtpDTO confirmOtpDTO)
    {
        return await _otpService.VerifyOtpAsync(confirmOtpDTO);
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
        _tokenService.DeleteTokenCookie(_httpContextAccessor.HttpContext);
        await _signInManager.SignOutAsync();
    }
}
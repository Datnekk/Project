using be.Dtos.Auth;

namespace be.Repositories;

public interface IAuthService
{
    Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO);
    Task<(AuthResponseDTO User, string EmailConfirmationToken)> RegisterAsync(RegisterDTO registerDTO);
    Task<bool> ConfirmEmailAsync(ConfirmEmailDTO confirmEmailDTO);
    Task LogoutAsync();
}
namespace be.Dtos.Auth;

public class RefreshTokenRequestDTO
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
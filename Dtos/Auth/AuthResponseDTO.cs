namespace be.Dtos.Auth;

public class AuthResponseDTO
{
    public bool IsAuthSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
}
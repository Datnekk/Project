namespace be.Dtos.Auth;

public class ConfirmEmailDTO
{
    public int UserId { get; set; }
    public string Token { get; set; }
}
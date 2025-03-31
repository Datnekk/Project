namespace be.Dtos.Users
{
    public class UserReadDTO
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public IList<string>? Role { get; set; }
    }
}
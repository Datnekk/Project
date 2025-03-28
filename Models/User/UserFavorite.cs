using System.ComponentModel.DataAnnotations.Schema;

namespace be.Models
{
    public class UserFavorite
    {
        public int UserId { get; set; }
        public string FavoriteId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
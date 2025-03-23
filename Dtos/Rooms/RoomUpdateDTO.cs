using System.ComponentModel.DataAnnotations;

namespace be.Dtos.Rooms
{
    public class RoomUpdateDTO
    {   
        [Required]
        [MaxLength(50, ErrorMessage = "Room name must be at most 50 characters long")]
        public string RoomName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Location must be at most 50 characters long")]
        public string Location { get; set; }

        [MaxLength(200, ErrorMessage = "Description must be at most 200 characters long")]
        public string Description { get; set; }

        [Required]
        [Range(1, 1000000)]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
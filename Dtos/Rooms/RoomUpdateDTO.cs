using System.ComponentModel.DataAnnotations;

namespace be.Dtos.Rooms
{
    public class RoomUpdateDTO
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Title must be at most 100 characters long")]
        public string Title { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Location must be at most 100 characters long")]
        public string Location { get; set; }

        [MaxLength(500, ErrorMessage = "Description must be at most 500 characters long")]
        public string? Description { get; set; }

        [Required]
        public string ImageSrc { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Room count must be at least 1")]
        public int RoomCount { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Bathroom count must be at least 1")]
        public int BathroomCount { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Guest count must be at least 1")]
        public int GuestCount { get; set; }

        [Required]
        [Range(1, 1000000)]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}

using System.ComponentModel.DataAnnotations;

namespace be.Dtos.Rooms
{
    public class RoomUpdateDTO
    {   
        [Required]
        public string Type { get; set; }
        [Required]
        [Range(1, 1000000)]
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}
namespace be.Dtos.Rooms
{
    public class RoomReadDTO
    {
        public int RoomId { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string? Description { get; set; }
        public string ImageSrc { get; set; }
        public int CategoryId { get; set; }
        public int RoomCount { get; set; }
        public int BathroomCount { get; set; }
        public int GuestCount { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

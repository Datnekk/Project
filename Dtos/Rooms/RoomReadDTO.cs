namespace be.Dtos.Rooms
{
    public class RoomReadDTO
    {
        public int RoomId { get; set; }
        public string RoomType { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}
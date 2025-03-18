using be.Models.enums;

namespace be.Helpers
{
    public class RoomQueryObject
    {
        public string? Location { get; set; }
        public bool? IsAvailable { get; set; }
        public RoomType? RoomType { get; set; } 
        public decimal? MinPrice { get; set; }  
        public decimal? MaxPrice { get; set; }  

    }
}
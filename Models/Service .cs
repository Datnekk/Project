using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace be.Models
{
    public class Service 
    {
    [Key]
    public int ServiceId { get; set; }
    public string ServiceName { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
    public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using be.Models.enums;

namespace be.Models
{
    public class Payment 
    {
    [Key]
    public int PaymentId { get; set; }
    public int BookingId { get; set; }
    public int UserId { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    [ForeignKey("BookingId")]
    public Booking Booking { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; }
    }
}
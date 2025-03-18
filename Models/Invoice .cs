using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace be.Models
{
    public class Invoice 
    {
    [Key]
    public int InvoiceId { get; set; }
    public int BookingId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }
    public DateTime InvoiceDate { get; set; }
    }
}
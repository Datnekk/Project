using System.ComponentModel.DataAnnotations;

namespace be.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    
    public ICollection<Room> Rooms { get; set; } = [];
}
using System.ComponentModel.DataAnnotations;

namespace be.Dtos.Category;

public class CategoryCreateDTO
{
    [Required]
    [MaxLength(20, ErrorMessage = "Name must be at most 20 characters long")]
    public string Name { get; set; }
    [MaxLength(100, ErrorMessage = "Description must be at most 100 characters long")]
    public string Description { get; set; }
    [MaxLength(20, ErrorMessage = "Icon must be at most 20 characters long")]
    public string Icon { get; set; }  
}

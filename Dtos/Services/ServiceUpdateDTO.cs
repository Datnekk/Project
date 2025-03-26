using System.ComponentModel.DataAnnotations;

namespace be.Dtos.Services;

public class ServiceUpdateDTO
{
    [Required]
    [MinLength(3, ErrorMessage = "Service name is too short.")]
    [MaxLength(50, ErrorMessage = "Service name is too long.")]
    public string ServiceName { get; set; }

    [MaxLength(50, ErrorMessage = "Description is too long.")]
    public string Description { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace outlay_mvc.Dtoes;

public class CreateOutlayDto
{
    [Required]
    [StringLength(30, ErrorMessage = "The {0} must be between 4 and 20 characters", MinimumLength = 4)]
    public string? Name { get; set; }
    [StringLength(30, ErrorMessage = "The {0} must be between 4 and 30 characters", MinimumLength = 4)]
    public string? Description { get; set; }
    [Required]
    public int? Cost { get; set; }
    [Required]
    public int CategoryId { get; set; }
}

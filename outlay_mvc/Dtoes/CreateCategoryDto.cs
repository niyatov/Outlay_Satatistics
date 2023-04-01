using System.ComponentModel.DataAnnotations;

namespace outlay_mvc.Dtoes;

public class CreateCategoryDto
{
    [Required]
    [StringLength(30, ErrorMessage = "The {0} must be between 4 and 20 characters", MinimumLength = 4)]
    public string? Name { get; set; }
    [StringLength(30, ErrorMessage = "The {0} must be between 4 and 30 characters", MinimumLength = 4)]
    public string? Description { get; set; }
    [StringLength(30, ErrorMessage = "The {0} must be between 4 and 40 characters", MinimumLength = 4)]
    public string? Key { get; set; }
}

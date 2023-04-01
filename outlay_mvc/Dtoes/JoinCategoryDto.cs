using System.ComponentModel.DataAnnotations;

namespace outlay_mvc.Dtoes;

public class JoinCategoryDto
{
    [Required]
    [StringLength(30, ErrorMessage = "The {0} must be between 4 and 20 characters", MinimumLength = 4)]
    public string? Name { get; set; }
    [Required, MaxLength(40), MinLength(4)]
    public string? Key { get; set; }
}

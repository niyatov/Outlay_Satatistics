using System.ComponentModel.DataAnnotations;

namespace outlay_mvc.Dtoes;

public class RegisterDto
{
    [Required, MaxLength(20), MinLength(4)]
    public string? Username { get; set; }

    [Required, DataType(DataType.EmailAddress), MaxLength(64), MinLength(4)]
    public string? Email { get; set; }
    [Required, DataType(DataType.Password), MinLength(6)]
    public string? Password { get; set; }
    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    public string? ConfirmPassword { get; set; }
}
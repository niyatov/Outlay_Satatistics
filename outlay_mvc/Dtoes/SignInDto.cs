using System.ComponentModel.DataAnnotations;

namespace outlay_mvc.Dtoes;

public class SignInDto
{
    [Required, MinLength(4), MaxLength(64)]
    public string? UsernameOrEmail { get; set; }
    [Required, DataType(DataType.Password), MinLength(6)]
    public string? Password { get; set; }
}
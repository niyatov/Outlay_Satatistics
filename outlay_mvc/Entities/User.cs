using Microsoft.AspNetCore.Identity;

namespace outlay_mvc.Entities;

public class User : IdentityUser<int>
{
    public virtual List<UserCategory>? UserCategories { get; set; }
    public virtual List<Outlay>? Outlays { get; set; }

}

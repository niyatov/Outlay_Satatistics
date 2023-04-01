namespace outlay_mvc.Entities;

public class Category : EntityBase
{
    public string? Name { get; set; }
    public string? Key { get; set; }
    public virtual List<UserCategory>? UserCategories { get; set; }
    public bool IsPrivate { get; set; }
    public virtual List<Outlay>? Outlays { get; set; }
}

namespace outlay_mvc.Models;

public class GetCategoryVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime? LastUpdateAt { get; set; }
    public string? Key { get; set; }
    public bool IsAdmin { get; set; }
    public string Type { get; set; }
    public bool ShowKey { get; set; }
    public FilterGetOutlays FilterGetOutlays { get; set; }
}


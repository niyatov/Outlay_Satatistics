using outlay_mvc.Dtoes;
namespace outlay_mvc.Models;

public class GetCategoriesVM : PaginationParams
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsAdmin { get; set; }
    public int NumberOfOutlays { get; set; }
    public string Type { get; set; }
}

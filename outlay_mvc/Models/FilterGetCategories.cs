using outlay_mvc.Dtoes;
using outlay_mvc.Models;

public class FilterGetCategories
{
    public PaginationParams PaginationParams { get; set; }
    public List<GetCategoriesVM>? GetCategories { get; set; }
}

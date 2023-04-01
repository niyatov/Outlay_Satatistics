using outlay_mvc.Dtoes;

namespace outlay_mvc.Models;

public class FilterGetOutlays
{
    public PaginationParams PaginationParams { get; set; }
    public List<GetOutlaysVM>? Outlays { get; set; }
}

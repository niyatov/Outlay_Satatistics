using Microsoft.AspNetCore.Mvc;

namespace outlay_mvc.Filters;

public class OutlayFilterAttribute : TypeFilterAttribute
{
    public OutlayFilterAttribute(string position) : base(typeof(OutlayAttribute))
    {
        Arguments = new object[] { position };
    }
}
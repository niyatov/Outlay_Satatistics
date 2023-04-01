using Microsoft.AspNetCore.Mvc;

namespace outlay_mvc.Filters;

public class CategoryFilterAttribute : TypeFilterAttribute
{
    public CategoryFilterAttribute(string position) : base(typeof(CategoryAttribute))
    {
        Arguments = new object[] { position };
    }
}
namespace outlay_mvc.Dtoes;

public static class PagedListHelper
{
    public static Tuple<IEnumerable<T>?, bool, bool> ToPagedList<T>(this IEnumerable<T> source, PaginationParams pageParams)
    {
        pageParams ??= new PaginationParams();
        IEnumerable<T>? result;
        bool IsNext = true;
        bool IsBack = true;

        if ((pageParams.Page - 1) * pageParams.Size >= source.Count())
        {
            result = null;
            IsNext = false;

            if ((pageParams.Page - 2) * pageParams.Size >= source.Count())
                IsBack = false;
        }
        else if (pageParams.Page * pageParams.Size < source.Count())
        {
            result = source.Skip(pageParams.Size * (pageParams.Page - 1)).Take(pageParams.Size).ToList();

            if (pageParams.Page * pageParams.Size >= source.Count())
                IsNext = false;
        }
        else
        {
            result = source.Skip(pageParams.Size * (pageParams.Page - 1)).Take(source.Count() - (pageParams.Page - 1) * pageParams.Size).ToList();
            IsNext = false;
        }

        if (pageParams.Page == 1)
            IsBack = false;

        return Tuple.Create(result, IsBack, IsNext);
    }
}

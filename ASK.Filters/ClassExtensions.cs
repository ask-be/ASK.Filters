namespace ASK.Filters;

public static class ClassExtensions
{
    public static IQueryable<TSource> ApplyFilter<TSource>(
        this IQueryable<TSource> source,
        Filter filter)
    {
        return source.Where(filter.CreateExpression<TSource>());
    }

    public static IEnumerable<TSource> ApplyFilter<TSource>(
        this IEnumerable<TSource> source,
        Filter filter)
    {
        return source.Where(filter.CreateExpression<TSource>().Compile());
    }
}
namespace ASK.Filters;

public static class ClassExtensions
{
    public static IQueryable<TSource> ApplyFilter<TSource>(
        this IQueryable<TSource> source,
        Filter filter)
    {
        return ApplyFilter(source, filter, FilterEvaluator<TSource>.Default);
    }

    public static IQueryable<TSource> ApplyFilter<TSource>(
        this IQueryable<TSource> source,
        Filter filter,
        FilterEvaluator<TSource> filterEvaluator)
    {
        return source.Where(filterEvaluator.Evaluate(filter));
    }

    public static IEnumerable<TSource> ApplyFilter<TSource>(
        this IEnumerable<TSource> source,
        Filter filter)
    {
        return ApplyFilter(source, filter, FilterEvaluator<TSource>.Default);
    }

    public static IEnumerable<TSource> ApplyFilter<TSource>(
        this IEnumerable<TSource> source,
        Filter filter,
        FilterEvaluator<TSource> filterEvaluator)
    {
        return source.Where(filterEvaluator.Evaluate(filter).Compile());
    }
}
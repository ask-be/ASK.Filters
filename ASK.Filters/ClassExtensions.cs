using System.Linq.Expressions;

namespace ASK.Filters;

public static class ClassExtensions
{
    public static IQueryable<TSource> ApplyFilter<TSource>(
        this IQueryable<TSource> source,
        FilterPropertyType filterPropertyType)
    {
        return ApplyFilter(source, filterPropertyType, FilterEvaluator.Default);
    }

    public static IQueryable<TSource> ApplyFilter<TSource>(
        this IQueryable<TSource> source,
        FilterPropertyType filterPropertyType,
        FilterEvaluator filterEvaluator)
    {
        return source.Where(filterEvaluator.GetExpression<TSource>(filterPropertyType));
    }

    public static IEnumerable<TSource> ApplyFilter<TSource>(
        this IEnumerable<TSource> source,
        FilterPropertyType filterPropertyType)
    {
        return ApplyFilter(source, filterPropertyType, FilterEvaluator.Default);
    }

    public static IEnumerable<TSource> ApplyFilter<TSource>(
        this IEnumerable<TSource> source,
        FilterPropertyType filterPropertyType,
        FilterEvaluator filterEvaluator)
    {
        return source.Where(filterEvaluator.GetExpression<TSource>(filterPropertyType).Compile());
    }
}
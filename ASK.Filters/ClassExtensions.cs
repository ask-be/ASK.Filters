using System.Linq.Expressions;

namespace ASK.Filters;

public delegate Expression CreatePropertyExpression(ParameterExpression parameter, string propertyName);

public static class ClassExtensions
{
    public static IQueryable<TSource> ApplyFilter<TSource>(
        this IQueryable<TSource> source,
        Filter filter)
    {
        return ApplyFilter(source, filter, FilterEvaluator.Default);
    }

    public static IQueryable<TSource> ApplyFilter<TSource>(
        this IQueryable<TSource> source,
        Filter filter,
        FilterEvaluator filterEvaluator)
    {
        return source.Where(filterEvaluator.GetExpression<TSource>(filter));
    }

    public static IEnumerable<TSource> ApplyFilter<TSource>(
        this IEnumerable<TSource> source,
        Filter filter)
    {
        return ApplyFilter(source, filter, FilterEvaluator.Default);
    }

    public static IEnumerable<TSource> ApplyFilter<TSource>(
        this IEnumerable<TSource> source,
        Filter filter,
        FilterEvaluator filterEvaluator)
    {
        return source.Where(filterEvaluator.GetExpression<TSource>(filter).Compile());
    }
}
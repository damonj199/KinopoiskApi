using System.Linq.Expressions;
using System.Reflection;

using KinopoiskDB.Core.Enum;

namespace KinopoiskDB.Dal.PostgreSQL.Repository;

public static class QueryableExtensions
{
    public static IQueryable<T> Sort<T>(this IQueryable<T> source, string fieldName, SortOrder order)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(fieldName) || typeof(T).GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase) is null)
        {
            fieldName = "KinopoiskId";
        }

        var method = order is SortOrder.ASC
            ? nameof(Queryable.OrderBy)
            : nameof(Queryable.OrderByDescending);

        var parameter = Expression.Parameter(typeof(T), "t");
        var property = Expression.Property(parameter, fieldName);
        var lambda = Expression.Lambda(property, parameter);

        var call = Expression.Call(typeof(Queryable), method, [source.ElementType, lambda.Body.Type], [source.Expression, lambda]);

        return source.Provider.CreateQuery<T>(call);
    }

    public static IQueryable<T> Paginate<T>(this IQueryable<T> source, int page, int pageSize) where T : class
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Номер страницы должен быть 1 или больше");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Размер страницы должен быть 1 или больше");

        return source.Skip((page - 1) * pageSize).Take(pageSize);
    }
}

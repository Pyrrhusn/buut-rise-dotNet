using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Rise.Domain.Common;

namespace Rise.Services.Pagination;

public class OrderingExpression<TEntity, TProperty> where TEntity : Entity
{
    public required Expression<Func<TEntity, TProperty>> OrderLambda { get; set; }
    public bool IsDescending { get; set; } = false;

    public static IQueryable<TEntity> GetOrderedQuery(IQueryable<TEntity> query, List<OrderingExpression<TEntity, object>> orderingExpressions, Expression<Func<TEntity, bool>>? filterLambda = null, bool inverseOrder = false)
    {
        if (filterLambda is not null)
        {
            query = query.Where(filterLambda);
        }

        if (orderingExpressions is not null && orderingExpressions.Count > 0)
        {
            IOrderedQueryable<TEntity> orderedQuery = (orderingExpressions[0].IsDescending || inverseOrder)
                ? query.OrderByDescending(orderingExpressions[0].OrderLambda)
                : query.OrderBy(orderingExpressions[0].OrderLambda);

            foreach (var expression in orderingExpressions.Skip(1))
            {
                orderedQuery = (expression.IsDescending || inverseOrder)
                    ? orderedQuery.ThenByDescending(expression.OrderLambda)
                    : orderedQuery.ThenBy(expression.OrderLambda);
            }

            return orderedQuery;
        }
        else
        {
            return query.OrderBy(e => e.Id);
        }
    }
}

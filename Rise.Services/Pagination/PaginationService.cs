using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Common;
using Rise.Shared;
using Rise.Shared.Pagination;

namespace Rise.Services.Pagination;

public static class PaginationService
{
    public async static Task<ItemsPageDto<TDto>> GetPaginatedResultsAsync<TEntity, TDto>(
        IQueryable<TEntity> queryableDbSet,
        List<OrderingExpression<TEntity, object>> orderingExpressions,
        Expression<Func<TEntity, TDto>> projection,
        int? cursor,
        bool? isNextPage,
        int pageSize,
        Expression<Func<TEntity, bool>>? filterLambda = null)
        where TEntity : Entity
        where TDto : BaseDto
    {
        var entitiesQuery = OrderingExpression<TEntity, object>.GetOrderedQuery(queryableDbSet, orderingExpressions, filterLambda);
        bool isDescendingOrder = orderingExpressions.Count > 0 && orderingExpressions[0].IsDescending;

        int takeAmount = pageSize + 1;

        if (cursor is not null)
        {
            if (isNextPage == true)
            {
                if (isDescendingOrder)
                {
                    entitiesQuery = entitiesQuery.Where(e => e.Id < cursor);
                }
                else
                {
                    entitiesQuery = entitiesQuery.Where(e => e.Id > cursor);
                }
            }
            else
            {
                if (isDescendingOrder)
                {
                    entitiesQuery = OrderingExpression<TEntity, object>.GetOrderedQuery(
                        entitiesQuery,
                        orderingExpressions,
                        e => e.Id > cursor,
                        true);
                }
                else
                {
                    entitiesQuery = OrderingExpression<TEntity, object>.GetOrderedQuery(
                        entitiesQuery,
                        orderingExpressions,
                        e => e.Id < cursor,
                        true);
                }
                takeAmount = pageSize;
            }
        }

        entitiesQuery = entitiesQuery.Take(takeAmount);
        if (isNextPage == false && cursor is not null)
        {
            entitiesQuery = entitiesQuery.Reverse();
        }

        var queriedEntities = await entitiesQuery.Select(projection).ToListAsync();

        if (queriedEntities.Count == 0)
        {
            return new ItemsPageDto<TDto>
            {
                Data = [],
                IsFirstPage = true,
                NextId = null,
                PreviousId = null
            };
        }

        bool isFirstPage = !cursor.HasValue ||
            (cursor.HasValue &&
                queriedEntities.FirstOrDefault()?.Id ==
                    OrderingExpression<TEntity, object>
                        .GetOrderedQuery(queryableDbSet, orderingExpressions, filterLambda)
                            .FirstOrDefault()?.Id);

        bool hasNextPage = queriedEntities.Count > pageSize ||
            (cursor is not null && isNextPage == false);

        if (queriedEntities.Count > pageSize)
        {
            queriedEntities.RemoveAt(queriedEntities.Count - 1);
        }

        int? nextId = hasNextPage
            ? queriedEntities.LastOrDefault()?.Id
            : null;

        int? previousId = queriedEntities.Count > 0 && !isFirstPage
            ? queriedEntities.FirstOrDefault()?.Id
            : null;

        return new ItemsPageDto<TDto>
        {
            Data = queriedEntities,
            IsFirstPage = isFirstPage,
            NextId = nextId,
            PreviousId = previousId
        };
    }
}

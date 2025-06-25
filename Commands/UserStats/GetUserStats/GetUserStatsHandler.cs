using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LiveCodingApp.Data.Context;
using LiveCodingApp.Data.Models;

namespace LiveCodingApp.Commands.GetUserStats;

public class GetUserStatsHandler : IRequestHandler<GetUserStatsRequest, IEnumerable<UserStatsResult>>
{
    private readonly AppDbContext _context;

    public GetUserStatsHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserStatsResult>> Handle(GetUserStatsRequest request, CancellationToken cancellationToken)
    {
        if (request == null || request.UserIds == null || !request.UserIds.Any())
        {
            return Enumerable.Empty<UserStatsResult>();
        }
        if (request.DateFrom > request.DateTo)
        {
            throw new ArgumentException("DateFrom must be less than or equal to DateTo");
        }

        var userIds = request.UserIds.ToList();
        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(cancellationToken);
        if (!users.Any())
            return Enumerable.Empty<UserStatsResult>();

        var products = await _context.Products
            .Where(p => userIds.Contains(p.UserId))
            .Select(p => new { p.Id, p.UserId })
            .ToListAsync(cancellationToken);

        var results = new List<UserStatsResult>();
        const int batchSize = 100; // Можно подобрать оптимальный размер

        foreach (var batch in Batch(users, batchSize))
        {
            var batchUserIds = batch.Select(u => u.Id).ToList();

            // Получаем действия только для пользователей из текущего батча
            var userActions = await _context.UserActions
                .Where(ua => batchUserIds.Contains(ua.UserId) &&
                             ua.CreatedAt >= request.DateFrom &&
                             ua.CreatedAt <= request.DateTo)
                .ToListAsync(cancellationToken);

            var productViews = userActions
                .Where(ua => ua.ActionTypeId == ActionType.View.Id && ua.EntityTypeId == EntityType.Product.Id)
                .ToList();

            foreach (var user in batch)
            {
                var userProductIds = products.Where(p => p.UserId == user.Id).Select(p => p.Id).ToList();
                var userActionsForUser = userActions.Where(ua => ua.UserId == user.Id).ToList();

                // Метрики, количество считаем сразу
                var comments = userActionsForUser.Count(ua => ua.ActionTypeId == ActionType.Comment.Id);
                var serviceReactions = userActionsForUser.Count(ua => ua.ActionTypeId == ActionType.React.Id && ua.EntityTypeId == EntityType.Service.Id);
                var addToCart = userActionsForUser.Count(ua => ua.ActionTypeId == ActionType.AddToCard.Id);
                var uniqueProductViews = userActionsForUser
                    .Where(ua => ua.ActionTypeId == ActionType.View.Id && ua.EntityTypeId == EntityType.Product.Id)
                    .Select(ua => ua.EntityId)
                    .Distinct()
                    .Count();

                // сколько других пользователей посмотрели товары пользователя
                var userProductView = userProductIds.Count == 0 ? 0 :
                    productViews
                        .Where(ua => userProductIds.Contains(ua.EntityId) && ua.UserId != user.Id)
                        .Select(ua => ua.UserId)
                        .Distinct()
                        .Count();

                // Группировка по датам для периодов
                var dailyStats = userActionsForUser.Any()
                    ? userActionsForUser
                        .GroupBy(ua => DateOnly.FromDateTime(ua.CreatedAt))
                        .Select(g => new UserStatsDetail
                        {
                            Date = g.Key,
                            Comments = g.Count(ua => ua.ActionTypeId == ActionType.Comment.Id),
                            ServiceReactions = g.Count(ua => ua.ActionTypeId == ActionType.React.Id && ua.EntityTypeId == EntityType.Service.Id),
                            AddToCartEvents = g.Count(ua => ua.ActionTypeId == ActionType.AddToCard.Id),
                            UniqueProductViews = g.Where(ua => ua.ActionTypeId == ActionType.View.Id && ua.EntityTypeId == EntityType.Product.Id)
                                                 .Select(ua => ua.EntityId)
                                                 .Distinct()
                                                 .Count(),
                            UserProductView = 0 // Эта метрика считается по пользователю, не по дню
                        })
                        .OrderBy(s => s.Date)
                        .ToList()
                    : new List<UserStatsDetail>();

                var totalStats = new UserStats
                {
                    Comments = comments,
                    ServiceReactions = serviceReactions,
                    AddToCartEvents = addToCart,
                    UniqueProductViews = uniqueProductViews,
                    UserProductView = userProductView
                };

                results.Add(new UserStatsResult
                {
                    Email = user.Email,
                    Periods = dailyStats,
                    Total = totalStats
                });
            }
        }

        return results;
    }

    // Batch helper
    private static IEnumerable<List<T>> Batch<T>(IEnumerable<T> source, int size)
    {
        var batch = new List<T>(size);
        foreach (var item in source)
        {
            batch.Add(item);
            if (batch.Count == size)
            {
                yield return batch;
                batch = new List<T>(size);
            }
        }
        if (batch.Count > 0)
            yield return batch;
    }
}
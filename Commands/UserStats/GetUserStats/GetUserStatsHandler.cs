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
        var users = await _context.Users
            .Where(u => request.UserIds.Contains(u.Id))
            .ToListAsync(cancellationToken);

        var userActions = await _context.UserActions
            .Where(ua => request.UserIds.Contains(ua.UserId) && 
                        ua.CreatedAt >= request.DateFrom && 
                        ua.CreatedAt <= request.DateTo)
            .ToListAsync(cancellationToken);

        var results = new List<UserStatsResult>();

        foreach (var user in users)
        {
            var userActionsForUser = userActions.Where(ua => ua.UserId == user.Id).ToList();

            // Сколько пользователь оставил комментариев
            var commentActions = userActionsForUser.Where(ua => ua.ActionTypeId == ActionType.Comment.Id);

            // Сколько раз пользователь отреагировал на какие-либо услуги
            var serviceReactionActions = userActionsForUser.Where(ua => 
                ua.ActionTypeId == ActionType.React.Id && 
                ua.EntityTypeId == EntityType.Service.Id);

            // Сколько раз пользователь добавлял товар в корзину
            var addToCartActions = userActionsForUser.Where(ua => ua.ActionTypeId == ActionType.AddToCard.Id);

            // Сколько уникальных товаров отсмотрел пользователь
            var productViewActions = userActionsForUser.Where(ua => 
                ua.ActionTypeId == ActionType.View.Id && 
                ua.EntityTypeId == EntityType.Product.Id);

            // Сколько других пользователей посмотрели все товары указанного пользователя
            var userProductIds = await _context.Products
                .Where(p => p.UserId == user.Id)
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            var otherUsersViewingThisUserProducts = userActions
                .Where(ua => ua.UserId != user.Id && 
                           ua.ActionTypeId == ActionType.View.Id && 
                           ua.EntityTypeId == EntityType.Product.Id &&
                           userProductIds.Contains(ua.EntityId))
                .Select(ua => ua.UserId)
                .Distinct()
                .Count();

            // Group by date for periods
            var dailyStats = userActionsForUser
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
                    UserProductView = 0 // This is calculated per user, not per day
                })
                .OrderBy(s => s.Date)
                .ToList();

            var totalStats = new UserStats
            {
                Comments = commentActions.Count(),
                ServiceReactions = serviceReactionActions.Count(),
                AddToCartEvents = addToCartActions.Count(),
                UniqueProductViews = productViewActions.Select(ua => ua.EntityId).Distinct().Count(),
                UserProductView = otherUsersViewingThisUserProducts
            };

            results.Add(new UserStatsResult
            {
                Email = user.Email,
                Periods = dailyStats,
                Total = totalStats
            });
        }

        return results;
    }
}
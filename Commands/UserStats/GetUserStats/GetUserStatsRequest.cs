using System;
using System.Collections;
using System.Collections.Generic;
using MediatR;

namespace LiveCodingApp.Commands.GetUserStats;

public class UserStats
{
	// Сколько пользователь оставил комментариев
	public int Comments { get; set; }

	// Сколько раз пользователь отреагировал на какие-либо услуги
	public int ServiceReactions { get; set; }

	// Сколько раз пользователь добавлял товар в корзину
	public int AddToCartEvents { get; set; }

	// Сколько уникальных товаров отсмотрел пользователь
	public int UniqueProductViews { get; set; }

	// Сколько других пользователей посмотрели все товары указанного пользователя
	public int UserProductView { get; set; }
}
public class UserStatsDetail : UserStats
{
	public DateOnly Date { get; set; }
}
public class UserStatsResult
{
	public string Email { get; set; } = null!;
	public IEnumerable<UserStatsDetail> Periods { get; set; } = null!;
	public UserStats Total { get; set; } = null!;
}
public class GetUserStatsRequest : IRequest<IEnumerable<UserStatsResult>>
{
	public IEnumerable<Guid> UserIds { get; }
	public DateTime DateFrom { get; }

	public DateTime DateTo { get; }
	public GetUserStatsRequest(IEnumerable<Guid> userIds, DateTime dtFrom, DateTime dtTo)
	{
		this.UserIds = userIds;
		this.DateFrom = dtFrom;
		this.DateTo = dtTo;
	}
}
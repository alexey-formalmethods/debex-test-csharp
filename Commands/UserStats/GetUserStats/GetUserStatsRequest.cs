using System;
using MediatR;

public class GetUserStatsRequest: IRequest<UserStats>
{
	public Guid UserId { get; }
	public GetUserStatsRequest(Guid userId)
	{
		this.UserId = userId;
	}
}
using System.Collections.Generic;
using LiveCodingApp.Commands.GetUserStats;
using MediatR;

public class PrintUserStatsRequest: IRequest
{
	public IEnumerable<UserStatsResult> UserStats { get; }
	public PrintUserStatsRequest(IEnumerable<UserStatsResult> userStats)
	{
		this.UserStats = userStats;
	}
}
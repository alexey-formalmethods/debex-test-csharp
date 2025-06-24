using System;
using System.Threading;
using System.Threading.Tasks;
using LiveCodingApp.Commands.GetUserStats;
using LiveCodingApp.Commands.UserStats.PrintUserStats;
using MediatR;
using System.Linq;
using LiveCodingApp.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LiveCodingApp.Commands.CodingSession;

public class CodingSessionHandler : IRequestHandler<CodingSessionRequest>
{
	private readonly IMediator _mediator;
	private readonly AppDbContext _context;

	public CodingSessionHandler(IMediator mediator, AppDbContext context)
	{
		_mediator = mediator;
		_context = context;
	}

	public async Task Handle(CodingSessionRequest request, CancellationToken cancellationToken)
	{
		// Get all users from database
		var allUsers = await _context
			.Users
			.Select(u => u.Id)
			.ToListAsync(cancellationToken);
		
		var userStatsResults = await _mediator.Send(
			new GetUserStatsRequest(
				allUsers,
				DateTime.UtcNow.AddDays(-5),
				DateTime.UtcNow
			), cancellationToken
		);

		await _mediator.Send(
			new PrintUserStatsRequest(
				userStatsResults
			),
			cancellationToken
		);
	}
}
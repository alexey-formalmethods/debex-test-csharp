using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using LiveCodingApp.Commands.GetUserStats;

namespace LiveCodingApp.Commands.UserStats.PrintUserStats;

public class PrintUserStatsHandler : IRequestHandler<PrintUserStatsRequest>
{
	public Task Handle(PrintUserStatsRequest request, CancellationToken cancellationToken)
	{
		PrintTable(request.UserStats);
		return Task.CompletedTask;
	}

	private static void PrintTable(IEnumerable<UserStatsResult> results)
	{
		var resultsList = results.ToList();
		if (resultsList.Count == 0)
		{
			Console.WriteLine("No data available.");
			return;
		}

		// Get all unique dates across all users
		var allDates = resultsList
			.SelectMany(r => r.Periods.Select(p => p.Date))
			.Distinct()
			.OrderBy(d => d)
			.ToList();

		// Define metrics with updated names
		var metrics = new[] { "Comments", "ServiceReactions", "AddToCartEvents", "UniqueProductViews", "UserProductView" };

		// Calculate column widths
		var emailWidth = Math.Max(10, resultsList.Max(r => r.Email.Length) + 2);
		var metricWidth = Math.Max(15, metrics.Max(m => m.Length) + 2);
		var dateWidth = 12;
		var totalWidth = 8;

		// Print header
		PrintHeader(emailWidth, metricWidth, allDates, dateWidth, totalWidth);

		// Print data for each user
		foreach (var userResult in resultsList)
		{
			PrintUserData(userResult, allDates, emailWidth, metricWidth, dateWidth, totalWidth, metrics);
		}
	}

	private static void PrintHeader(int emailWidth, int metricWidth, List<DateOnly> allDates, int dateWidth, int totalWidth)
	{
		var sb = new StringBuilder();
		
		// Header row
		sb.Append("Email".PadRight(emailWidth));
		sb.Append("Metric".PadRight(metricWidth));
		
		foreach (var date in allDates)
		{
			sb.Append(date.ToString("MM/dd").PadRight(dateWidth));
		}
		sb.Append("Total".PadRight(totalWidth));
		
		Console.WriteLine(sb.ToString());
		
		// Separator line
		var separatorLength = emailWidth + metricWidth + (allDates.Count * dateWidth) + totalWidth;
		Console.WriteLine(new string('-', separatorLength));
	}

	private static void PrintUserData(UserStatsResult userResult, List<DateOnly> allDates, int emailWidth, int metricWidth, int dateWidth, int totalWidth, string[] metrics)
	{
		var periodsDict = userResult.Periods.ToDictionary(p => p.Date, p => p);

		for (int i = 0; i < metrics.Length; i++)
		{
			var sb = new StringBuilder();
			
			// Email column (only show on first metric row)
			if (i == 0)
				sb.Append(userResult.Email.PadRight(emailWidth));
			else
				sb.Append("".PadRight(emailWidth));
			
			// Metric name
			sb.Append(metrics[i].PadRight(metricWidth));
			
			// Date columns
			foreach (var date in allDates)
			{
				var value = 0;
				if (periodsDict.TryGetValue(date, out var period))
				{
					value = metrics[i] switch
					{
						"Comments" => period.Comments,
						"ServiceReactions" => period.ServiceReactions,
						"AddToCartEvents" => period.AddToCartEvents,
						"UniqueProductViews" => period.UniqueProductViews,
						"UserProductView" => period.UserProductView,
						_ => 0
					};
				}
				sb.Append(value.ToString().PadRight(dateWidth));
			}
			
			// Total column
			var total = metrics[i] switch
			{
				"Comments" => userResult.Total.Comments,
				"ServiceReactions" => userResult.Total.ServiceReactions,
				"AddToCartEvents" => userResult.Total.AddToCartEvents,
				"UniqueProductViews" => userResult.Total.UniqueProductViews,
				"UserProductView" => userResult.Total.UserProductView,
				_ => 0
			};
			sb.Append(total.ToString().PadRight(totalWidth));
			
			Console.WriteLine(sb.ToString());
		}
		
		// Empty line between users
		Console.WriteLine();
	}
}
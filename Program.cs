using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LiveCodingApp.Data.Context;
using LiveCodingApp.Data;
using MediatR;
using System.Threading;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = "Server=liseller-prod.postgres.database.azure.com;Port=5432;Database=livecoding;Uid=livecoder;Pwd=LC21341da#;Timeout = 40;SslMode=Require;";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    DbSeed.SeedData(context);
}

//  ------- ЗАДАНИЕ --------
using (var scope = app.Services.CreateScope())
{
    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    await mediator.Send(
        new CodingSessionRequest(),
        CancellationToken.None
    );
}
//  ------- /ЗАДАНИЕ --------
// await app.RunAsync();

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LiveCodingApp.Data.Context;
using LiveCodingApp.Data;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = "Server=localhost;Port=5433;Database=lc;Uid=postgres;Pwd=mysecretpassword;Timeout = 15;";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

// Ensure database is created and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    DbSeed.SeedData(context);
}

Console.WriteLine("Live Coding App started!");
Console.WriteLine("Database connection configured for PostgreSQL");
Console.WriteLine("MediatR configured and ready");
Console.WriteLine("Database seeded with initial data");

await app.RunAsync();
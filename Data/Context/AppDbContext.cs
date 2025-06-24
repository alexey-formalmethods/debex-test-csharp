using Microsoft.EntityFrameworkCore;
using LiveCodingApp.Data.Models;

namespace LiveCodingApp.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<UserAction> UserActions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ActionTypeEntity> ActionTypes { get; set; }
    public DbSet<EntityTypeEntity> EntityTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<UserAction>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
        });

        modelBuilder.Entity<ActionTypeEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.ToTable("ActionTypes");
        });

        modelBuilder.Entity<EntityTypeEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.ToTable("EntityTypes");
        });
    }
}
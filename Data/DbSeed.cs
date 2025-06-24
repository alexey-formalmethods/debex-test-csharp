using System;
using System.Linq;
using LiveCodingApp.Data.Context;
using LiveCodingApp.Data.Models;

namespace LiveCodingApp.Data;

public static class DbSeed
{
    public static void SeedData(AppDbContext context)
    {
        if (context.Users.Any())
        {
            return; // DB has been seeded
        }

        // Seed ActionTypes
        var actionTypes = new[]
        {
            new ActionTypeEntity { Id = ActionType.View.Id, Name = ActionType.View.Name },
            new ActionTypeEntity { Id = ActionType.AddToCard.Id, Name = ActionType.AddToCard.Name },
            new ActionTypeEntity { Id = ActionType.Comment.Id, Name = ActionType.Comment.Name },
            new ActionTypeEntity { Id = ActionType.React.Id, Name = ActionType.React.Name },
            new ActionTypeEntity { Id = ActionType.Bookmark.Id, Name = ActionType.Bookmark.Name }
        };

        context.ActionTypes.AddRange(actionTypes);
        context.SaveChanges();

        // Seed EntityTypes
        var entityTypes = new[]
        {
            new EntityTypeEntity { Id = EntityType.Product.Id, Name = EntityType.Product.Name },
            new EntityTypeEntity { Id = EntityType.Service.Id, Name = EntityType.Service.Name }
        };

        context.EntityTypes.AddRange(entityTypes);
        context.SaveChanges();

        // Seed Users
        var users = new[]
        {
            new User { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Email = "john.doe@example.com" },
            new User { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Email = "jane.smith@example.com" },
            new User { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Email = "bob.wilson@example.com" }
        };

        context.Users.AddRange(users);
        context.SaveChanges();

        // Seed Products
        var products = new[]
        {
            new Product { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Name = "Laptop", Price = 999.99m, UserId = users[0].Id },
            new Product { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Name = "Smartphone", Price = 599.99m, UserId = users[1].Id },
            new Product { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Name = "Headphones", Price = 199.99m, UserId = users[2].Id }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        // Seed Services
        var services = new[]
        {
            new Service { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Name = "Web Development", Price = 2500.00m, UserId = users[0].Id },
            new Service { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Name = "Mobile App Design", Price = 1800.00m, UserId = users[1].Id },
            new Service { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Name = "SEO Optimization", Price = 800.00m, UserId = users[2].Id },
            new Service { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Name = "Digital Marketing", Price = 1200.00m, UserId = users[0].Id },
            new Service { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Name = "Data Analysis", Price = 1500.00m, UserId = users[1].Id }
        };

        context.Services.AddRange(services);
        context.SaveChanges();

        // Seed UserActions using static values
        var productEntityTypeId = EntityType.Product.Id;
        var serviceEntityTypeId = EntityType.Service.Id;
        var viewActionTypeId = ActionType.View.Id;
        var addToCartActionTypeId = ActionType.AddToCard.Id;
        var commentActionTypeId = ActionType.Comment.Id;
        var reactActionTypeId = ActionType.React.Id;

        var userActions = new[]
        {
            // Product views
            new UserAction 
            { 
                Id = Guid.NewGuid(), 
                CreatedAt = DateTime.UtcNow, 
                UserId = users[0].Id, 
                ActionTypeId = viewActionTypeId,
                StatusId = Guid.NewGuid(),
                EntityId = products[0].Id,
                EntityTypeId = productEntityTypeId,
                Credits = 10
            },
            // Add to cart
            new UserAction 
            { 
                Id = Guid.NewGuid(), 
                CreatedAt = DateTime.UtcNow, 
                UserId = users[1].Id, 
                ActionTypeId = addToCartActionTypeId,
                StatusId = Guid.NewGuid(),
                EntityId = products[1].Id,
                EntityTypeId = productEntityTypeId,
                Credits = 25
            },
            // Comments
            new UserAction 
            { 
                Id = Guid.NewGuid(), 
                CreatedAt = DateTime.UtcNow, 
                UserId = users[2].Id, 
                ActionTypeId = commentActionTypeId,
                StatusId = Guid.NewGuid(),
                EntityId = products[0].Id,
                EntityTypeId = productEntityTypeId,
                Credits = 5
            },
            // Service reactions
            new UserAction 
            { 
                Id = Guid.NewGuid(), 
                CreatedAt = DateTime.UtcNow, 
                UserId = users[0].Id, 
                ActionTypeId = reactActionTypeId,
                StatusId = Guid.NewGuid(),
                EntityId = services[0].Id,
                EntityTypeId = serviceEntityTypeId,
                Credits = 15
            },
            new UserAction 
            { 
                Id = Guid.NewGuid(), 
                CreatedAt = DateTime.UtcNow, 
                UserId = users[1].Id, 
                ActionTypeId = reactActionTypeId,
                StatusId = Guid.NewGuid(),
                EntityId = services[1].Id,
                EntityTypeId = serviceEntityTypeId,
                Credits = 20
            },
            // Cross-user product views (for UserProductView metric)
            new UserAction 
            { 
                Id = Guid.NewGuid(), 
                CreatedAt = DateTime.UtcNow, 
                UserId = users[1].Id, 
                ActionTypeId = viewActionTypeId,
                StatusId = Guid.NewGuid(),
                EntityId = products[0].Id, // user[0]'s product viewed by user[1]
                EntityTypeId = productEntityTypeId,
                Credits = 8
            },
            new UserAction 
            { 
                Id = Guid.NewGuid(), 
                CreatedAt = DateTime.UtcNow, 
                UserId = users[2].Id, 
                ActionTypeId = viewActionTypeId,
                StatusId = Guid.NewGuid(),
                EntityId = products[0].Id, // user[0]'s product viewed by user[2]
                EntityTypeId = productEntityTypeId,
                Credits = 12
            }
        };

        context.UserActions.AddRange(userActions);
        context.SaveChanges();
    }
}
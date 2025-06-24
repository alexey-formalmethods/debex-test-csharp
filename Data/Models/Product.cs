using System;

namespace LiveCodingApp.Data.Models;

public class Product
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Guid UserId { get; set; }
}
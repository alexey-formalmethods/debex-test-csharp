using System;

namespace LiveCodingApp.Data.Models;

public class User
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Email { get; set; } = string.Empty;
}
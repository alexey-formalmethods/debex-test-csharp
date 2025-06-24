using System;

namespace LiveCodingApp.Data.Models;

public class UserAction
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
    public Guid ActionTypeId { get; set; }
    public Guid StatusId { get; set; }
    public Guid EntityId { get; set; }
    public Guid EntityTypeId { get; set; }
    public int Credits { get; set; }
}
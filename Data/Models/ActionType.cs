using System;

namespace LiveCodingApp.Data.Models;

public static class ActionType
{
    public static class View
    {
        public static readonly Guid Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly string Name = "View";
    }

    public static class AddToCard
    {
        public static readonly Guid Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly string Name = "AddToCard";
    }

    public static class Comment
    {
        public static readonly Guid Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        public static readonly string Name = "Comment";
    }

    public static class React
    {
        public static readonly Guid Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
        public static readonly string Name = "React";
    }

    public static class Bookmark
    {
        public static readonly Guid Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
        public static readonly string Name = "Bookmark";
    }
}
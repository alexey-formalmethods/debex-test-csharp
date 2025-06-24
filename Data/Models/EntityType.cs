using System;

namespace LiveCodingApp.Data.Models;

public static class EntityType
{
    public static class Product
    {
        public static readonly Guid Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        public static readonly string Name = "Product";
    }

    public static class Service
    {
        public static readonly Guid Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        public static readonly string Name = "Service";
    }
}
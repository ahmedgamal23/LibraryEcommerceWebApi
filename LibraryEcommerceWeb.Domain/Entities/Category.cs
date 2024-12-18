﻿namespace LibraryEcommerceWeb.Domain.Entities
{
    public class Category
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;

        public ICollection<Product>? Products { get; set; }
    }
}
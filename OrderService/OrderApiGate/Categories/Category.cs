﻿namespace OrderApiGate.Categories
{
    public class Category : WriteCategory
    {
        public long CategoryId { get; set; }
        public bool IsActive { get; set; }
    }
}

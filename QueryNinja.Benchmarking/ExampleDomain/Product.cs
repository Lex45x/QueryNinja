using System.Collections.Generic;

namespace QueryNinja.Benchmarking.ExampleDomain
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<ProductVariant> ProductVariants { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
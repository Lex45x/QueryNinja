namespace QueryNinja.Benchmarking.ExampleDomain
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public VariantKind Kind { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal Discount { get; set; }
    }
}
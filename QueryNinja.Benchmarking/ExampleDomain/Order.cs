using System.Collections.Generic;

namespace QueryNinja.Benchmarking.ExampleDomain
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<ProductVariant> OrderedProducts { get; set; }

        public decimal TotalProductsPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal ShippingPrice { get; set; }

        public OrderStatus Status { get; set; }
    }
}
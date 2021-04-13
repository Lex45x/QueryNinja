using System.Collections.Generic;

namespace QueryNinja.Benchmarking.ExampleDomain
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
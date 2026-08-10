using System;

namespace Vegetable.Entities
{
    public class OwnerCustomer
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }

        public Guid OwnerId { get; set; }
        public Owner Owner { get; set; }
    }
}

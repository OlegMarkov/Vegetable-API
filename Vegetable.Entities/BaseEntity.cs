using System;

namespace Vegetable.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public Owner Owner { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime CreatedDateUTC { get; set; }
    }
}

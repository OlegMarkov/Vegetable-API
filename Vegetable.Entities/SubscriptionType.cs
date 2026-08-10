using System.ComponentModel.DataAnnotations;

namespace Vegetable.Entities
{
    public class SubscriptionType
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsDefault { get; set; }
    }
}

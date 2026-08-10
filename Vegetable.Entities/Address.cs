using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vegetable.Entities
{
    public class Address : BaseEntity
    {
        [MaxLength(50)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string State { get; set; }

        [MaxLength(30)]
        public string City { get; set; }

        [MaxLength(10)]
        public string PostalCode { get; set; }

        [MaxLength(50)]
        public string Street { get; set; }

        [MaxLength(50)]
        public string Unit { get; set; }

        [MaxLength(50)]
        public string Points { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Vegetable.Entities
{
    public class PhoneNumber
    {
        public Guid Id { get; set; }

        [MaxLength(20)]
        public string Number { get; set; }
        public PhoneNumberType Type { get; set; }

        public Guid? OwnerId { get; set; }
        public Owner Owner { get; set; }

        public Guid? CustomerId { get; set; }
        public Customer Customer { get; set; }

    }

    public enum PhoneNumberType : byte
    {
        Classic = 1,
        Mobile = 2
    }
}

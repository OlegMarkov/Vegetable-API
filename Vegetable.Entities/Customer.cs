using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Vegetable.Entities
{

    public class Customer : BaseEntity
    {

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        public string Phone { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        public string Notes { get; set; }

        public bool IsDeleted { get; set; }
        
        public ICollection<Reservation> Reservations { get; set; }

        public ICollection<Image> Images { get; set; }

        public long? ChatId { get; set; }

        [MaxLength(2)]
        public string ChatLanguage { get; set; }

        public bool SendConfirmationSms { get; set; }
    }
}

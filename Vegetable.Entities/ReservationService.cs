using System;

namespace Vegetable.Entities
{
    public class ReservationService
    {
        public Guid ReservationId { get; set; }
        public Reservation Reservation { get; set; }

        public Guid ServiceId { get; set; }
        public Service Service { get; set; }
    }
}

using System;

namespace Vegetable.API.ViewModels
{
    public class ImageInfo
    {
        public string ImageBase64 { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsPrimary { get; set; }

        public Guid? ReservationId { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? ServiceId { get; set; }

        public Guid? EmployeeId { get; set; }
    }
}

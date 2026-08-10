using System;

namespace Vegetable.Entities
{
    public class EmployeeService
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public Guid ServiceId { get; set; }
        public Service Service { get; set; }
    }
}

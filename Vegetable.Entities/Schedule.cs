using System;
using System.Collections.Generic;

namespace Vegetable.Entities
{ 
    public class Schedule : BaseEntity
    {
        public Schedule() { 
        }

        public Schedule(ScheduleType type)
        {
            switch (type)
            {
                case ScheduleType.Week:
                    ScheduleStartDate = DateTime.Today;
                    ScheduleEndDate = DateTime.Today.AddYears(10);
                    OnDays = 5;
                    OffDays = 2;
                    ScheduleType = ScheduleType.Week;
                    ScheduleOnDays = new List<ScheduleOnDay>() {
                    new ScheduleOnDay{
                        Sequence = 1,
                        WorkStartTime = new TimeSpan(9, 0, 0),
                        WorkEndTime = new TimeSpan(18, 0, 0),
                        EnableBreakTime = true,
                        BreakStartTime = new TimeSpan(13, 0, 0),
                        BreakEndTime = new TimeSpan(14, 0, 0),
                        IsEnabled = true
                    },
                    new ScheduleOnDay{
                        Sequence = 2,
                        WorkStartTime = new TimeSpan(9, 0, 0),
                        WorkEndTime = new TimeSpan(18, 0, 0),
                        EnableBreakTime = true,
                        BreakStartTime = new TimeSpan(13, 0, 0),
                        BreakEndTime = new TimeSpan(14, 0, 0),
                        IsEnabled = true
                    },
                    new ScheduleOnDay{
                        Sequence = 3,
                        WorkStartTime = new TimeSpan(9, 0, 0),
                        WorkEndTime = new TimeSpan(18, 0, 0),
                        EnableBreakTime = true,
                        BreakStartTime = new TimeSpan(13, 0, 0),
                        BreakEndTime = new TimeSpan(14, 0, 0),
                        IsEnabled = true
                    },
                    new ScheduleOnDay{
                        Sequence = 4,
                        WorkStartTime = new TimeSpan(9, 0, 0),
                        WorkEndTime = new TimeSpan(18, 0, 0),
                        EnableBreakTime = true,
                        BreakStartTime = new TimeSpan(13, 0, 0),
                        BreakEndTime = new TimeSpan(14, 0, 0),
                        IsEnabled = true
                    },
                    new ScheduleOnDay{
                        Sequence = 5,
                        WorkStartTime = new TimeSpan(9, 0, 0),
                        WorkEndTime = new TimeSpan(18, 0, 0),
                        EnableBreakTime = true,
                        BreakStartTime = new TimeSpan(13, 0, 0),
                        BreakEndTime = new TimeSpan(14, 0, 0),
                        IsEnabled = true
                    },
                    new ScheduleOnDay{
                        Sequence = 6,
                        WorkStartTime = new TimeSpan(9, 0, 0),
                        WorkEndTime = new TimeSpan(18, 0, 0),
                        EnableBreakTime = true,
                        BreakStartTime = new TimeSpan(13, 0, 0),
                        BreakEndTime = new TimeSpan(14, 0, 0),
                        IsEnabled = false
                    },
                    new ScheduleOnDay{
                        Sequence = 7,
                        WorkStartTime = new TimeSpan(9, 0, 0),
                        WorkEndTime = new TimeSpan(18, 0, 0),
                        EnableBreakTime = true,
                        BreakStartTime = new TimeSpan(13, 0, 0),
                        BreakEndTime = new TimeSpan(14, 0, 0),
                        IsEnabled = false
                    }
                };
                    break;
            }

            
        }

        public DateTime ScheduleStartDate { get; set; }

        public DateTime? ScheduleEndDate { get; set; }

        public ushort OnDays { get; set; }

        public ushort OffDays { get; set; }

        public ScheduleType ScheduleType { get; set; }

        public Guid EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public ICollection<ScheduleOnDay> ScheduleOnDays { get; set; }
    }

    public enum ScheduleType
    {
        Week = 0,
        Switch = 1,
        Custom = 2
    }
    
}

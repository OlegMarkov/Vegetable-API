using System;

namespace Vegetable.Entities
{
    public class ScheduleOnDay : BaseEntity
    {

        public ScheduleOnDay()
        {
            WorkStartTime = new TimeSpan(9, 0, 0);
            WorkEndTime = new TimeSpan(18, 0, 0);
            BreakStartTime = new TimeSpan(13, 0, 0);
            BreakEndTime = new TimeSpan(14, 0, 0);            
        }

        public Guid ScheduleId { get; set; }

        public Schedule Schedule { get; set; }

        public ushort Sequence { get; set; }

        public TimeSpan WorkStartTime { get; set; }

        public TimeSpan WorkEndTime { get; set; }

        public TimeSpan BreakStartTime { get; set; }

        public TimeSpan BreakEndTime { get; set; }

        public bool EnableBreakTime { get; set; }

        public bool IsEnabled { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Vegetable.Core.Models
{
    public class TimeLine : List<DateTimeRange>
    {
        public TimeLine(DateTimeRange range)
        {
            Add(range);
        }
        public TimeLine(DateTime start, DateTime end)
        {
            Add(new DateTimeRange(start, end));
        }
        public void Substruct(DateTimeRange range)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                var type = range.GetIntersectionType(this[i]);
                if (type == IntersectionType.RangesEqauled || type == IntersectionType.ContainedInRange)
                {
                    RemoveAt(i);
                }
                else if (type == IntersectionType.StartsInRange)
                {
                    DateTimeRange dtr = this[i];
                    dtr.Start = range.End;
                    this[i] = dtr;
                }
                else if (type == IntersectionType.EndsInRange)
                {
                    DateTimeRange dtr = this[i];
                    dtr.End = range.Start;
                    this[i] = dtr;
                }
                else if (type == IntersectionType.ContainsRange)
                {
                    DateTimeRange dtr = this[i];
                    Insert(i + 1, new DateTimeRange(range.End, dtr.End));
                    dtr.End = range.Start;
                    this[i] = dtr;
                }
            }
        }

        public void Substruct(IEnumerable<DateTimeRange> ranges)
        {
            foreach (var range in ranges) Substruct(range);
        }

        public List<DateTime> GetAvalibleSlots(int stepInMinutes, int durationInMinutes)
        {
            List<DateTime> slots = new List<DateTime>();
            foreach (var (Start, End) in this)
            {
                var startSlot = Start;
                var endSlot = startSlot.AddMinutes(durationInMinutes);

                while (endSlot <= End)
                {
                    slots.Add(startSlot);

                    startSlot = startSlot.AddMinutes(stepInMinutes);
                    endSlot = startSlot.AddMinutes(durationInMinutes);
                }
            }
            return slots;
        }
    }
}

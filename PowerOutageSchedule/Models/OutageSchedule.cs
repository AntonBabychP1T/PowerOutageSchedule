namespace PowerOutageSchedule.Models
{
    using System.Collections.Generic;

    public class OutageSchedule
    {
        public int GroupNumber { get; set; }
        public List<TimeInterval> OutageIntervals { get; set; }
    }
}

namespace PowerOutageSchedule.Models
{
    public class OutageSchedule
    {
        public int GroupNumber { get; set; }
        public List<TimeInterval> OutageIntervals { get; set; }
    }

    public class TimeInterval
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
    }

}

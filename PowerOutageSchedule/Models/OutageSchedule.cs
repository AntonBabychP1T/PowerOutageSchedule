namespace PowerOutageSchedule.Models
{
    public class OutageSchedule
    {
        public int GroupNumber { get; set; }
        public List<TimeInterval> OutageIntervals { get; set; }
    }

    public class TimeInterval
    {
        public string Start { get; set; }
        public string End { get; set; }
    }
}

namespace PowerOutageSchedule.Models
{
    public class GroupOutageStatus
    {
        public int GroupNumber { get; set; }
        public bool HasPower { get; set; }
        public string Message { get; set; }
    }
}

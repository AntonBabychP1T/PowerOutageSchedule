namespace PowerOutageSchedule.DTOs
{
    using System.Collections.Generic;

    public class EditScheduleDto
    {
        public int GroupNumber { get; set; }
        public List<TimeIntervalDto> OutageIntervals { get; set; }
    }
}

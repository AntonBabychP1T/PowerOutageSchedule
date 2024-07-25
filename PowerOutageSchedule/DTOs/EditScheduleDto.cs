namespace PowerOutageSchedule.DTOs
{
    using System.Collections.Generic;

    public class EditScheduleDto
    {
        public List<TimeIntervalDto> OutageIntervals { get; set; }
    }
}

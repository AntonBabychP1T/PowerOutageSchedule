namespace PowerOutageSchedule.DTOs
{
    using Microsoft.AspNetCore.Http;

    public class ImportScheduleDto
    {
        public IFormFile File { get; set; }
    }
}

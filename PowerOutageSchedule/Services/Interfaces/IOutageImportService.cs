namespace PowerOutageSchedule.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerOutageSchedule.Models;

    public interface IOutageImportService
    {
        Task<IEnumerable<OutageSchedule>> ImportSchedulesAsync(string filePath);
    }
}

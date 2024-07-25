namespace PowerOutageSchedule.Services
{
    using PowerOutageSchedule.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IOutageService
    {
        Task<IEnumerable<OutageSchedule>> ImportSchedulesAsync(string filePath);
        IEnumerable<OutageSchedule> GetCurrentOutages();
        OutageSchedule GetScheduleByGroup(int groupNumber);
        void EditSchedule(int groupNumber, OutageSchedule schedule);
        Task ExportSchedulesAsync(string filePath);
    }
}

namespace PowerOutageSchedule.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IOutageExportService
    {
        Task ExportSchedulesAsync(string filePath);
    }
}

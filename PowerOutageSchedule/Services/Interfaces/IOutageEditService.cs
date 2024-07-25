namespace PowerOutageSchedule.Services.Interfaces
{
    using PowerOutageSchedule.Models;

    public interface IOutageEditService
    {
        void EditSchedule(int groupNumber, OutageSchedule schedule);
    }
}

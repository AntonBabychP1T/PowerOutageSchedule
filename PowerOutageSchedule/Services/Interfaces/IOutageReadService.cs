namespace PowerOutageSchedule.Services.Interfaces
{
    using System.Collections.Generic;
    using PowerOutageSchedule.Models;

    public interface IOutageReadService
    {
        IEnumerable<OutageSchedule> GetCurrentOutages();
        OutageSchedule GetScheduleByGroup(int groupNumber);
        GroupOutageStatus GetGroupOutageStatus(int groupNumber);
    }

}

using PowerOutageSchedule.Models;
using PowerOutageSchedule.Services.Interfaces;

public class OutageReadEditService : IOutageReadService, IOutageEditService
{
    private readonly DataStore _dataStore;
    private readonly ILogger<OutageReadEditService> _logger;

    public OutageReadEditService(DataStore dataStore, ILogger<OutageReadEditService> logger)
    {
        _dataStore = dataStore;
        _logger = logger;
    }

    public IEnumerable<OutageSchedule> GetCurrentOutages()
    {
        var now = DateTime.Now.TimeOfDay;
        return _dataStore.Schedules.Where(s => s.OutageIntervals.Any(i =>
        {
            var start = TimeSpan.Parse(i.Start);
            var end = TimeSpan.Parse(i.End);
            return start <= now && end >= now;
        })).ToList();
    }

    public OutageSchedule GetScheduleByGroup(int groupNumber)
    {
        return _dataStore.Schedules.FirstOrDefault(s => s.GroupNumber == groupNumber);
    }

    public GroupOutageStatus GetGroupOutageStatus(int groupNumber)
    {
        var schedule = GetScheduleByGroup(groupNumber);
        if (schedule == null)
        {
            return null;
        }

        var now = DateTime.Now.TimeOfDay;
        var currentInterval = schedule.OutageIntervals
            .FirstOrDefault(interval => TimeSpan.Parse(interval.Start) <= now && TimeSpan.Parse(interval.End) >= now);

        if (currentInterval != null)
        {
            var timeUntilPowerOn = TimeSpan.Parse(currentInterval.End) - now;
            return new GroupOutageStatus
            {
                GroupNumber = groupNumber,
                HasPower = false,
                Message = $"Power is off. Time until power on: {timeUntilPowerOn.ToString(@"hh\:mm\:ss")}"
            };
        }
        else
        {
            var nextOutage = schedule.OutageIntervals
                .Where(interval => TimeSpan.Parse(interval.Start) > now)
                .OrderBy(interval => TimeSpan.Parse(interval.Start))
                .FirstOrDefault();

            if (nextOutage != null)
            {
                var timeUntilNextOutage = TimeSpan.Parse(nextOutage.Start) - now;
                return new GroupOutageStatus
                {
                    GroupNumber = groupNumber,
                    HasPower = true,
                    Message = $"Power is on. Time until next outage: {timeUntilNextOutage.ToString(@"hh\:mm\:ss")}"
                };
            }
            else
            {
                return new GroupOutageStatus
                {
                    GroupNumber = groupNumber,
                    HasPower = true,
                    Message = "Power is on. No more outages today."
                };
            }
        }
    }

    public void EditSchedule(int groupNumber, OutageSchedule schedule)
    {
        var existingSchedule = _dataStore.Schedules.FirstOrDefault(s => s.GroupNumber == groupNumber);
        if (existingSchedule != null)
        {
            existingSchedule.OutageIntervals = schedule.OutageIntervals;
            _logger.LogInformation("Edited schedule for group {GroupNumber}: {Schedule}", groupNumber, existingSchedule);
        }
        else
        {
            _dataStore.Schedules.Add(schedule);
            _logger.LogInformation("Added new schedule for group {GroupNumber}: {Schedule}", groupNumber, schedule);
        }
    }
}

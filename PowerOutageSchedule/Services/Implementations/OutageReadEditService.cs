namespace PowerOutageSchedule.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;
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
            var currentOutages = _dataStore.Schedules.Where(s => s.OutageIntervals.Any(i =>
            {
                var start = TimeSpan.Parse(i.Start);
                var end = TimeSpan.Parse(i.End);
                return start <= now && end >= now;
            })).ToList();

            _logger.LogInformation("Current outages: {CurrentOutages}", currentOutages);
            return currentOutages;
        }

        public OutageSchedule GetScheduleByGroup(int groupNumber)
        {
            var schedule = _dataStore.Schedules.FirstOrDefault(s => s.GroupNumber == groupNumber);
            _logger.LogInformation("Schedule for group {GroupNumber}: {Schedule}", groupNumber, schedule);
            return schedule;
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

}

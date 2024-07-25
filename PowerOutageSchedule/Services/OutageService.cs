namespace PowerOutageSchedule.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    using PowerOutageSchedule.Models;

    public class OutageService : IOutageService
    {
        private List<OutageSchedule> _schedules = new();

        public async Task<IEnumerable<OutageSchedule>> ImportSchedulesAsync(string filePath)
        {
            var lines = await File.ReadAllLinesAsync(filePath);
            var schedules = new List<OutageSchedule>();

            foreach (var line in lines)
            {
                var parts = line.Split('.');
                if (parts.Length != 2)
                {
                    throw new FormatException("Invalid format");
                }

                if (!int.TryParse(parts[0], out int groupNumber))
                {
                    throw new FormatException("Invalid group number");
                }

                var intervals = parts[1].Split(';')
                    .Select(i =>
                    {
                        var times = i.Split('-');
                        if (times.Length != 2 || !TimeSpan.TryParse(times[0], out TimeSpan start) || !TimeSpan.TryParse(times[1], out TimeSpan end))
                        {
                            throw new FormatException("Invalid time interval");
                        }
                        return new TimeInterval { Start = start, End = end };
                    })
                    .ToList();

                schedules.Add(new OutageSchedule { GroupNumber = groupNumber, OutageIntervals = intervals });
            }

            _schedules = schedules;
            return _schedules;
        }

        public IEnumerable<OutageSchedule> GetCurrentOutages()
        {
            var now = DateTime.Now.TimeOfDay;
            return _schedules.Where(s => s.OutageIntervals.Any(i => i.Start <= now && i.End >= now));
        }

        public OutageSchedule GetScheduleByGroup(int groupNumber)
        {
            return _schedules.FirstOrDefault(s => s.GroupNumber == groupNumber);
        }

        public void EditSchedule(int groupNumber, OutageSchedule schedule)
        {
            var existingSchedule = _schedules.FirstOrDefault(s => s.GroupNumber == groupNumber);
            if (existingSchedule != null)
            {
                existingSchedule.OutageIntervals = schedule.OutageIntervals;
            }
            else
            {
                _schedules.Add(schedule);
            }
        }

        public async Task ExportSchedulesAsync(string filePath)
        {
            var json = JsonSerializer.Serialize(_schedules);
            await File.WriteAllTextAsync(filePath, json);
        }
    }
}

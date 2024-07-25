namespace PowerOutageSchedule.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using PowerOutageSchedule.Models;
    using PowerOutageSchedule.Services.Interfaces;

    public class OutageImportExportService : IOutageImportService, IOutageExportService
    {
        private readonly DataStore _dataStore;
        private readonly ILogger<OutageImportExportService> _logger;

        public OutageImportExportService(DataStore dataStore, ILogger<OutageImportExportService> logger)
        {
            _dataStore = dataStore;
            _logger = logger;
        }

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
                        if (times.Length != 2 || !TimeSpan.TryParse(times[0], out _) || !TimeSpan.TryParse(times[1], out _))
                        {
                            throw new FormatException("Invalid time interval");
                        }
                        return new TimeInterval { Start = times[0], End = times[1] };
                    })
                    .ToList();

                schedules.Add(new OutageSchedule { GroupNumber = groupNumber, OutageIntervals = intervals });
            }

            _dataStore.Schedules = schedules;
            _logger.LogInformation("Imported schedules: {Schedules}", schedules);
            return _dataStore.Schedules;
        }

        public async Task ExportSchedulesAsync(string filePath)
        {
            var json = JsonSerializer.Serialize(_dataStore.Schedules);
            await File.WriteAllTextAsync(filePath, json);
        }
    }

}

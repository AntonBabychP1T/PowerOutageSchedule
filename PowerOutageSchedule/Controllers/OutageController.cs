namespace PowerOutageSchedule.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using PowerOutageSchedule.DTOs;
    using PowerOutageSchedule.Models;
    using PowerOutageSchedule.Services.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    public class OutageController : ControllerBase
    {
        private readonly IOutageImportService _importService;
        private readonly IOutageExportService _exportService;
        private readonly IOutageReadService _readService;
        private readonly IOutageEditService _editService;

        public OutageController(IOutageImportService importService, IOutageExportService exportService, IOutageReadService readService, IOutageEditService editService)
        {
            _importService = importService;
            _exportService = exportService;
            _readService = readService;
            _editService = editService;
        }

        [HttpPost("import")]
        [SwaggerOperation(Summary = "Imports outage schedules from a file", Description = "Uploads a .txt file containing outage schedules")]
        [SwaggerResponse(StatusCodes.Status200OK, "Schedules imported successfully", typeof(IEnumerable<OutageSchedule>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid file format or empty file")]
        public async Task<IActionResult> ImportSchedules([FromForm] ImportScheduleDto importScheduleDto)
        {
            var file = importScheduleDto.File;

            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty");
            }

            var filePath = Path.GetTempFileName();

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                var schedules = await _importService.ImportSchedulesAsync(filePath);
                System.IO.File.Delete(filePath);
                return Ok(schedules);
            }
            catch (FormatException ex)
            {
                System.IO.File.Delete(filePath); 
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("current")]
        [SwaggerOperation(Summary = "Gets current outages", Description = "Returns the list of groups with current outages")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns current outages", typeof(IEnumerable<OutageSchedule>))]
        public IActionResult GetCurrentOutages()
        {
            var outages = _readService.GetCurrentOutages();
            return Ok(outages);
        }

        [HttpGet("group/{groupNumber}")]
        [SwaggerOperation(Summary = "Gets schedule for a specific group", Description = "Returns the outage schedule for the specified group number")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the outage schedule for the specified group", typeof(OutageSchedule))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Group not found")]
        public IActionResult GetScheduleByGroup(int groupNumber)
        {
            var schedule = _readService.GetScheduleByGroup(groupNumber);
            if (schedule == null)
            {
                return NotFound("Group not found");
            }
            return Ok(schedule);
        }

        [HttpPut("group/{groupNumber}")]
        [SwaggerOperation(Summary = "Edits the schedule for a specific group", Description = "Edits the outage schedule for the specified group number")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Schedule updated successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The schedule field is required")]
        public IActionResult EditSchedule(int groupNumber, [FromBody] EditScheduleDto scheduleDto)
        {
            if (scheduleDto == null)
            {
                return BadRequest("The schedule field is required.");
            }

            var schedule = new OutageSchedule
            {
                GroupNumber = groupNumber, 
                OutageIntervals = scheduleDto.OutageIntervals.Select(i => new TimeInterval { Start = i.Start, End = i.End }).ToList()
            };

            _editService.EditSchedule(groupNumber, schedule);
            return NoContent();
        }


        [HttpGet("export")]
        [SwaggerOperation(Summary = "Exports all schedules", Description = "Exports all outage schedules to a JSON file")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the JSON file with all schedules", typeof(FileResult))]
        public async Task<IActionResult> ExportSchedules()
        {
            var filePath = Path.GetTempFileName();
            await _exportService.ExportSchedulesAsync(filePath);

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            System.IO.File.Delete(filePath); 

            return File(fileBytes, "application/octet-stream", "schedules.json");
        }
    }
}

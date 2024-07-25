namespace PowerOutageSchedule.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using PowerOutageSchedule.Models;
    using PowerOutageSchedule.Services;

    [Route("api/[controller]")]
    [ApiController]
    public class OutageController : ControllerBase
    {
        private readonly IOutageService _outageService;

        public OutageController(IOutageService outageService)
        {
            _outageService = outageService;
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportSchedules(IFormFile file)
        {
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
                var schedules = await _outageService.ImportSchedulesAsync(filePath);
                System.IO.File.Delete(filePath); // Видалити тимчасовий файл після імпорту
                return Ok(schedules);
            }
            catch (FormatException ex)
            {
                System.IO.File.Delete(filePath); // Видалити тимчасовий файл у разі помилки
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("current")]
        public IActionResult GetCurrentOutages()
        {
            var outages = _outageService.GetCurrentOutages();
            return Ok(outages);
        }

        [HttpGet("group/{groupNumber}")]
        public IActionResult GetScheduleByGroup(int groupNumber)
        {
            var schedule = _outageService.GetScheduleByGroup(groupNumber);
            if (schedule == null)
            {
                return NotFound("Group not found");
            }
            return Ok(schedule);
        }

        [HttpPut("group/{groupNumber}")]
        public IActionResult EditSchedule(int groupNumber, [FromBody] OutageSchedule schedule)
        {
            _outageService.EditSchedule(groupNumber, schedule);
            return NoContent();
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportSchedules()
        {
            var filePath = Path.GetTempFileName();
            await _outageService.ExportSchedulesAsync(filePath);

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            System.IO.File.Delete(filePath); // Видалити тимчасовий файл після експорту

            return File(fileBytes, "application/octet-stream", "schedules.json");
        }
    }
}

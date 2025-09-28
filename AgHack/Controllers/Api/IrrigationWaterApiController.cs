using Microsoft.AspNetCore.Mvc;
using AgHack.Services;

namespace AgHack.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class IrrigationWaterApiController : ControllerBase
    {
        private readonly IIrrigationWaterService _irrigationWaterService;

        public IrrigationWaterApiController(IIrrigationWaterService irrigationWaterService)
        {
            _irrigationWaterService = irrigationWaterService;
        }

        // GET: api/IrrigationWaterApi/departments
        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var result = await _irrigationWaterService.GetDepartmentsAsync();
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        // GET: api/IrrigationWaterApi/stations
        [HttpGet("stations")]
        public async Task<IActionResult> GetStations([FromQuery] int? deptId = null)
        {
            var result = await _irrigationWaterService.GetStationsAsync(deptId);
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        // GET: api/IrrigationWaterApi/stations/{id}
        [HttpGet("stations/{id}")]
        public async Task<IActionResult> GetStation(int id)
        {
            var result = await _irrigationWaterService.GetStationAsync(id);
            return result.Success 
                ? Ok(result) 
                : result.Message.Contains("找不到") 
                    ? NotFound(result) 
                    : StatusCode(500, result);
        }

        // GET: api/IrrigationWaterApi/monitoringpoints
        [HttpGet("monitoringpoints")]
        public async Task<IActionResult> GetMonitoringPoints([FromQuery] int? stationId = null, [FromQuery] int? deptId = null)
        {
            var result = await _irrigationWaterService.GetMonitoringPointsAsync(stationId, deptId);
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        // GET: api/IrrigationWaterApi/records
        [HttpGet("records")]
        public async Task<IActionResult> GetRecords(
            [FromQuery] int? mpId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100)
        {
            var result = await _irrigationWaterService.GetRecordsAsync(mpId, startDate, endDate, page, pageSize);
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        // GET: api/IrrigationWaterApi/statistics
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics(
            [FromQuery] int? mpId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _irrigationWaterService.GetStatisticsAsync(mpId, startDate, endDate);
            return result.Success ? Ok(result) : StatusCode(500, result);
        }
    }
}
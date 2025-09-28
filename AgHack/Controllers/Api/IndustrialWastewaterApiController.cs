using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgHack.Models;

namespace AgHack.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndustrialWastewaterApiController : ControllerBase
    {
        private readonly AgHackContext _context;

        public IndustrialWastewaterApiController(AgHackContext context)
        {
            _context = context;
        }

        // GET: api/IndustrialWastewaterApi/departments
        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                var departments = await _context.IWQ_Depts
                    .Select(d => new
                    {
                        d.DeptId,
                        d.DeptName
                    })
                    .ToListAsync();

                return Ok(departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得部門資料時發生錯誤", error = ex.Message });
            }
        }

        // GET: api/IndustrialWastewaterApi/stations
        [HttpGet("stations")]
        public async Task<IActionResult> GetStations([FromQuery] int? deptId = null)
        {
            try
            {
                var query = _context.IWQ_Sts
                    .Include(s => s.County)
                    .Include(s => s.Town)
                    .Include(s => s.Dept)
                    .AsQueryable();

                if (deptId.HasValue)
                    query = query.Where(s => s.DeptId == deptId.Value);

                var stations = await query
                    .Select(s => new
                    {
                        s.StId,
                        s.StName,
                        s.SiteAddress,
                        s.TWD97Lat,
                        s.TWD97Lon,
                        CountyName = s.County.CountyName,
                        TownName = s.Town.TownName,
                        DeptName = s.Dept.DeptName
                    })
                    .ToListAsync();

                return Ok(stations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得工業廢水測站資料時發生錯誤", error = ex.Message });
            }
        }

        // GET: api/IndustrialWastewaterApi/stations/{id}
        [HttpGet("stations/{id}")]
        public async Task<IActionResult> GetStation(int id)
        {
            try
            {
                var station = await _context.IWQ_Sts
                    .Include(s => s.County)
                    .Include(s => s.Town)
                    .Include(s => s.Dept)
                    .Where(s => s.StId == id)
                    .Select(s => new
                    {
                        s.StId,
                        s.StName,
                        s.SiteAddress,
                        s.TWD97Lat,
                        s.TWD97Lon,
                        CountyName = s.County.CountyName,
                        TownName = s.Town.TownName,
                        DeptName = s.Dept.DeptName
                    })
                    .FirstOrDefaultAsync();

                if (station == null)
                {
                    return NotFound(new { message = "找不到指定的工業廢水測站" });
                }

                return Ok(station);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得工業廢水測站資料時發生錯誤", error = ex.Message });
            }
        }

        // GET: api/IndustrialWastewaterApi/monitoringpoints
        [HttpGet("monitoringpoints")]
        public async Task<IActionResult> GetMonitoringPoints([FromQuery] int? stationId = null, [FromQuery] int? deptId = null)
        {
            try
            {
                var query = _context.IWQ_MPs
                    .Include(mp => mp.St)
                    .Include(mp => mp.Dept)
                    .AsQueryable();

                if (stationId.HasValue)
                    query = query.Where(mp => mp.StId == stationId.Value);

                if (deptId.HasValue)
                    query = query.Where(mp => mp.DeptId == deptId.Value);

                var monitoringPoints = await query
                    .Select(mp => new
                    {
                        mp.MPId,
                        mp.MPName,
                        StationName = mp.St.StName,
                        DeptName = mp.Dept.DeptName
                    })
                    .ToListAsync();

                return Ok(monitoringPoints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得監測點資料時發生錯誤", error = ex.Message });
            }
        }

        // GET: api/IndustrialWastewaterApi/records
        [HttpGet("records")]
        public async Task<IActionResult> GetRecords(
            [FromQuery] int? mpId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100)
        {
            try
            {
                var query = _context.IWQ_Records
                    .Include(r => r.MP)
                    .ThenInclude(mp => mp.St)
                    .AsQueryable();

                if (mpId.HasValue)
                    query = query.Where(r => r.MPId == mpId.Value);

                if (startDate.HasValue)
                    query = query.Where(r => r.SampleDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(r => r.SampleDate <= endDate.Value);

                var totalCount = await query.CountAsync();
                var records = await query
                    .OrderByDescending(r => r.SampleDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new
                    {
                        r.RecordId,
                        r.SampleDate,
                        r.PH,
                        r.Temp,
                        r.EC,
                        r.Note,
                        MonitoringPointName = r.MP.MPName,
                        StationName = r.MP.St.StName
                    })
                    .ToListAsync();

                return Ok(new
                {
                    data = records,
                    pagination = new
                    {
                        page,
                        pageSize,
                        totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得工業廢水監測資料時發生錯誤", error = ex.Message });
            }
        }

        // GET: api/IndustrialWastewaterApi/statistics
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics(
            [FromQuery] int? mpId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var query = _context.IWQ_Records.AsQueryable();

                if (mpId.HasValue)
                    query = query.Where(r => r.MPId == mpId.Value);

                if (startDate.HasValue)
                    query = query.Where(r => r.SampleDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(r => r.SampleDate <= endDate.Value);

                var statistics = await query
                    .GroupBy(r => 1)
                    .Select(g => new
                    {
                        Count = g.Count(),
                        PH = new
                        {
                            Average = g.Where(r => r.PH.HasValue).Average(r => r.PH),
                            Min = g.Where(r => r.PH.HasValue).Min(r => r.PH),
                            Max = g.Where(r => r.PH.HasValue).Max(r => r.PH)
                        },
                        Temperature = new
                        {
                            Average = g.Where(r => r.Temp.HasValue).Average(r => r.Temp),
                            Min = g.Where(r => r.Temp.HasValue).Min(r => r.Temp),
                            Max = g.Where(r => r.Temp.HasValue).Max(r => r.Temp)
                        },
                        EC = new
                        {
                            Average = g.Where(r => r.EC.HasValue).Average(r => r.EC),
                            Min = g.Where(r => r.EC.HasValue).Min(r => r.EC),
                            Max = g.Where(r => r.EC.HasValue).Max(r => r.EC)
                        }
                    })
                    .FirstOrDefaultAsync();

                return Ok(statistics ?? new { 
                    Count = 0, 
                    PH = new { Average = (decimal?)null, Min = (decimal?)null, Max = (decimal?)null },
                    Temperature = new { Average = (decimal?)null, Min = (decimal?)null, Max = (decimal?)null },
                    EC = new { Average = (decimal?)null, Min = (decimal?)null, Max = (decimal?)null }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得工業廢水統計資料時發生錯誤", error = ex.Message });
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgHack.Models;

namespace AgHack.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroundwaterApiController : ControllerBase
    {
        private readonly AgHackContext _context;

        public GroundwaterApiController(AgHackContext context)
        {
            _context = context;
        }

        // GET: api/GroundwaterApi/stations
        [HttpGet("stations")]
        public async Task<IActionResult> GetStations()
        {
            try
            {
                var stations = await _context.UG_Sts
                    .Include(s => s.County)
                    .Include(s => s.Town)
                    .Select(s => new
                    {
                        s.StId,
                        s.SiteId,
                        s.SiteName,
                        s.SiteEngName,
                        s.SiteAddress,
                        s.TWD97Lat,
                        s.TWD97Lon,
                        s.UGWDistName,
                        CountyName = s.County.CountyName,
                        TownName = s.Town.TownName
                    })
                    .ToListAsync();

                return Ok(stations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "���o�a�U��������Ʈɵo�Ϳ��~", error = ex.Message });
            }
        }

        // GET: api/GroundwaterApi/stations/{id}
        [HttpGet("stations/{id}")]
        public async Task<IActionResult> GetStation(int id)
        {
            try
            {
                var station = await _context.UG_Sts
                    .Include(s => s.County)
                    .Include(s => s.Town)
                    .Where(s => s.StId == id)
                    .Select(s => new
                    {
                        s.StId,
                        s.SiteId,
                        s.SiteName,
                        s.SiteEngName,
                        s.SiteAddress,
                        s.TWD97Lat,
                        s.TWD97Lon,
                        s.TWD97TM2X,
                        s.TWD97TM2Y,
                        s.UGWDistName,
                        CountyName = s.County.CountyName,
                        TownName = s.Town.TownName
                    })
                    .FirstOrDefaultAsync();

                if (station == null)
                {
                    return NotFound(new { message = "�䤣����w���a�U������" });
                }

                return Ok(station);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "���o�a�U��������Ʈɵo�Ϳ��~", error = ex.Message });
            }
        }

        // GET: api/GroundwaterApi/records
        [HttpGet("records")]
        public async Task<IActionResult> GetRecords(
            [FromQuery] int? stationId = null,
            [FromQuery] int? itemId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100)
        {
            try
            {
                var query = _context.UG_Records
                    .Include(r => r.St)
                    .Include(r => r.Item)
                    .ThenInclude(i => i.ItemCategory)
                    .AsQueryable();

                if (stationId.HasValue)
                    query = query.Where(r => r.StId == stationId.Value);

                if (itemId.HasValue)
                    query = query.Where(r => r.ItemId == itemId.Value);

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
                        r.ItemValue,
                        r.ItemValue_Num,
                        r.Note,
                        StationName = r.St.SiteName,
                        StationId = r.St.StId,
                        ItemName = r.Item.ItemName,
                        ItemUnit = r.Item.ItemUnit,
                        CategoryName = r.Item.ItemCategory.ItemCategoryName
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
                return StatusCode(500, new { message = "���o�a�U���ʴ���Ʈɵo�Ϳ��~", error = ex.Message });
            }
        }

        // GET: api/GroundwaterApi/items
        [HttpGet("items")]
        public async Task<IActionResult> GetItems()
        {
            try
            {
                var items = await _context.UG_Items
                    .Include(i => i.ItemCategory)
                    .Select(i => new
                    {
                        i.ItemId,
                        i.ItemName,
                        i.ItemEngName,
                        i.ItemEngabbreviation,
                        i.ItemUnit,
                        CategoryName = i.ItemCategory.ItemCategoryName,
                        CategoryEngName = i.ItemCategory.ItemCategoryEngName
                    })
                    .OrderBy(i => i.CategoryName)
                    .ThenBy(i => i.ItemName)
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "���o�a�U���ʴ����ظ�Ʈɵo�Ϳ��~", error = ex.Message });
            }
        }

        // GET: api/GroundwaterApi/statistics
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics(
            [FromQuery] int? stationId = null,
            [FromQuery] int? itemId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var query = _context.UG_Records.AsQueryable();

                if (stationId.HasValue)
                    query = query.Where(r => r.StId == stationId.Value);

                if (itemId.HasValue)
                    query = query.Where(r => r.ItemId == itemId.Value);

                if (startDate.HasValue)
                    query = query.Where(r => r.SampleDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(r => r.SampleDate <= endDate.Value);

                var numericRecords = query.Where(r => r.ItemValue_Num.HasValue);

                var statistics = await numericRecords
                    .GroupBy(r => 1)
                    .Select(g => new
                    {
                        Count = g.Count(),
                        Average = g.Average(r => r.ItemValue_Num),
                        Min = g.Min(r => r.ItemValue_Num),
                        Max = g.Max(r => r.ItemValue_Num)
                    })
                    .FirstOrDefaultAsync();

                return Ok(statistics ?? new { Count = 0, Average = (decimal?)null, Min = (decimal?)null, Max = (decimal?)null });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "���o�a�U���έp��Ʈɵo�Ϳ��~", error = ex.Message });
            }
        }
    }
}
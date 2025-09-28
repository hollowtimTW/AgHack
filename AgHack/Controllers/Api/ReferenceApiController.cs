using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgHack.Models;

namespace AgHack.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferenceApiController : ControllerBase
    {
        private readonly AgHackContext _context;

        public ReferenceApiController(AgHackContext context)
        {
            _context = context;
        }

        // GET: api/ReferenceApi/counties
        [HttpGet("counties")]
        public async Task<IActionResult> GetCounties()
        {
            try
            {
                var counties = await _context.Ref_Counties
                    .Select(c => new
                    {
                        c.CountyId,
                        c.CountyCode,
                        c.CountyName
                    })
                    .OrderBy(c => c.CountyCode)
                    .ToListAsync();

                return Ok(counties);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得縣市資料時發生錯誤", error = ex.Message });
            }
        }

        // GET: api/ReferenceApi/towns
        [HttpGet("towns")]
        public async Task<IActionResult> GetTowns([FromQuery] int? countyId = null)
        {
            try
            {
                var query = _context.Ref_Towns
                    .Include(t => t.County)
                    .AsQueryable();

                if (countyId.HasValue)
                    query = query.Where(t => t.CountyId == countyId.Value);

                var towns = await query
                    .Select(t => new
                    {
                        t.TownId,
                        t.TownCode,
                        t.TownName,
                        t.CountyId,
                        CountyName = t.County.CountyName
                    })
                    .OrderBy(t => t.CountyId)
                    .ThenBy(t => t.TownName)
                    .ToListAsync();

                return Ok(towns);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得鄉鎮市區資料時發生錯誤", error = ex.Message });
            }
        }

        // GET: api/ReferenceApi/basins
        [HttpGet("basins")]
        public async Task<IActionResult> GetBasins()
        {
            try
            {
                var basins = await _context.Ref_Basins
                    .Select(b => new
                    {
                        b.BasinId,
                        b.BasinCode,
                        b.BasinName
                    })
                    .OrderBy(b => b.BasinName)
                    .ToListAsync();

                return Ok(basins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得流域資料時發生錯誤", error = ex.Message });
            }
        }

        // GET: api/ReferenceApi/rivers
        [HttpGet("rivers")]
        public async Task<IActionResult> GetRivers()
        {
            try
            {
                var rivers = await _context.Ref_Rivers
                    .Select(r => new
                    {
                        r.RiverId,
                        r.RiverCode,
                        r.RiverName,
                        r.RiverEngName,
                        r.Description
                    })
                    .OrderBy(r => r.RiverName)
                    .ToListAsync();

                return Ok(rivers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得河川資料時發生錯誤", error = ex.Message });
            }
        }

        // GET: api/ReferenceApi/water-quality-categories
        [HttpGet("water-quality-categories")]
        public async Task<IActionResult> GetWaterQualityCategories()
        {
            try
            {
                var categories = await _context.WQ_ItemCategories
                    .Select(c => new
                    {
                        c.ItemCategoryId,
                        c.ItemCategoryName,
                        c.ItemCategoryEngName
                    })
                    .OrderBy(c => c.ItemCategoryName)
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得水質監測項目分類資料時發生錯誤", error = ex.Message });
            }
        }

        // GET: api/ReferenceApi/groundwater-categories
        [HttpGet("groundwater-categories")]
        public async Task<IActionResult> GetGroundwaterCategories()
        {
            try
            {
                var categories = await _context.UG_ItemCategories
                    .Select(c => new
                    {
                        c.ItemCategoryId,
                        c.ItemCategoryName,
                        c.ItemCategoryEngName
                    })
                    .OrderBy(c => c.ItemCategoryName)
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得地下水監測項目分類資料時發生錯誤", error = ex.Message });
            }
        }

        // GET: api/ReferenceApi/all-stations-summary
        [HttpGet("all-stations-summary")]
        public async Task<IActionResult> GetAllStationsSummary()
        {
            try
            {
                var wqStationsCount = await _context.WQ_Sts.CountAsync();
                var ugStationsCount = await _context.UG_Sts.CountAsync();
                var iwqStationsCount = await _context.IWQ_Sts.CountAsync();

                var summary = new
                {
                    WaterQualityStations = wqStationsCount,
                    GroundwaterStations = ugStationsCount,
                    IndustrialWastewaterStations = iwqStationsCount,
                    TotalStations = wqStationsCount + ugStationsCount + iwqStationsCount
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得測站統計資料時發生錯誤", error = ex.Message });
            }
        }
    }
}
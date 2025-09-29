using AgHack.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AgHack.Controllers.api
{
    [Route("api/UG")]
    [ApiController]
    public class UGController : ControllerBase
    {
        private readonly AgHackContext _context;

        public UGController(AgHackContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 取得所有啟用的地下水測站
        /// </summary>
        [HttpGet("GetAllUGSt")]
        public async Task<IActionResult> GetAllUGSt()
        {
            var excludedCountyIds = new HashSet<int> { 20, 21, 22 };

            var result = await _context.UG_Sts
                .AsNoTracking()
                .Where(st => st.StatusOfUse == 1 && (!st.CountyId.HasValue || !excludedCountyIds.Contains(st.CountyId.Value)))
                .Select(st => new
                {
                    st.StId,
                    st.SiteId,
                    st.SiteName,
                    st.TWD97Lat,
                    st.TWD97Lon
                })
                .ToListAsync();

            return Ok(result);
        }

        /// <summary>
        /// 取得某測站最新日期的所有監測紀錄
        /// </summary>
        [HttpGet("GetLatestRecordsById/{stId}")]
        public async Task<IActionResult> GetLatestRecordsById(int stId)
        {
            var stInfo = await _context.UG_Sts
                .AsNoTracking()
                .Where(st => st.StId == stId && st.StatusOfUse == 1)
                .Select(st => new
                {
                    st.StId,
                    st.SiteId,
                    st.SiteName,
                    st.SiteEngName,
                    st.UGWDistName,
                    st.SiteAddress
                })
                .FirstOrDefaultAsync();

            var latestDate = await _context.UG_Records
                  .AsNoTracking()
                  .Where(record => record.StId == stId)
                  .MaxAsync(record => record.SampleDate.Date);

            var latestRecords = await _context.UG_Records
                .AsNoTracking()
                .Include(s => s.Item)
                .Where(record => record.StId == stId && record.SampleDate.Date == latestDate)
                .OrderBy(p => p.Item.ItemId)
                .Select(p => new
                {
                    p.Item.ItemName,
                    p.Item.ItemEngabbreviation,
                    p.ItemValue,
                    p.Item.ItemUnit
                })
                .ToListAsync();

            if (latestRecords == null || !latestRecords.Any())
            {
                return NotFound("No records found.");
            }

            return Ok(new
            {
                StInfo = stInfo,
                SampleDate = latestDate.ToString("yyyy-MM-dd"),
                Records = latestRecords
            });
        }
    }
}

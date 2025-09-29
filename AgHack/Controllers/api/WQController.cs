using AgHack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AgHack.Controllers.api
{
    [Route("api/WQ")]
    [ApiController]
    public class WQController : ControllerBase
    {
        private readonly AgHackContext _context;

        public WQController(AgHackContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 取得所有啟用的水質測站
        /// </summary>
        [HttpGet("GetAllWQSt")]
        public async Task<IActionResult> GetAllWQSt()
        {
            var result = await _context.WQ_Sts
                .AsNoTracking() 
                .Where(st => st.StatusOfUse == 1) 
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
            var stInfo = await _context.WQ_Sts
                .AsNoTracking()
                .Include(s => s.Basin)
                .Where(st => st.StId == stId && st.StatusOfUse == 1)
                .Select(st => new
                {
                    st.StId,
                    st.SiteId,
                    st.SiteName,
                    st.SiteEngName,
                    st.Basin.BasinName,
                    st.SiteAddress
                })
                .FirstOrDefaultAsync();

            var latestDate = await _context.WQ_Records
                  .AsNoTracking()
                  .Where(record => record.StId == stId)
                  .MaxAsync(record => record.SampleDate.Date);

            var latestRecords = await _context.WQ_Records
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

            return Ok(new { 
                StInfo = stInfo,
                SampleDate = latestDate.ToString("yyyy-MM-dd"), 
                Records = latestRecords 
            });
        }
    }
}
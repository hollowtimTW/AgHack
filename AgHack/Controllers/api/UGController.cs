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
                    st.SiteName,
                    st.TWD97Lat,
                    st.TWD97Lon
                })
                .ToListAsync();

            return Ok(result);
        }


    }
}

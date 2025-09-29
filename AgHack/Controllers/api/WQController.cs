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
                    st.SiteName,
                    st.TWD97Lat,
                    st.TWD97Lon
                })
                .ToListAsync(); 

            return Ok(result);
        }


    }
}
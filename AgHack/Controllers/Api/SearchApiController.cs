using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgHack.Models;

namespace AgHack.Controllers.Api
{
    /// <summary>
    /// 搜尋與查詢 API 控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SearchApiController : ControllerBase
    {
        private readonly AgHackContext _context;

        public SearchApiController(AgHackContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 搜尋測站
        /// </summary>
        /// <param name="keyword">關鍵字（可選）</param>
        /// <param name="countyId">縣市ID（可選）</param>
        /// <param name="townId">鄉鎮市區ID（可選）</param>
        /// <param name="stationType">測站類型（可選）：WQ-水質、UG-地下水、IWQ-灌溉水質</param>
        /// <returns>符合條件的測站清單</returns>
        /// <response code="200">搜尋成功</response>
        /// <response code="500">伺服器內部錯誤</response>
        [HttpGet("stations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchStations(
            [FromQuery] string? keyword = null,
            [FromQuery] int? countyId = null,
            [FromQuery] int? townId = null,
            [FromQuery] string? stationType = null) // "WQ", "UG", "IWQ"
        {
            try
            {
                var results = new List<object>();

                // 搜尋水質測站
                if (string.IsNullOrEmpty(stationType) || stationType.ToUpper() == "WQ")
                {
                    var wqQuery = _context.WQ_Sts
                        .Include(s => s.County)
                        .Include(s => s.Town)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(keyword))
                        wqQuery = wqQuery.Where(s => s.SiteName.Contains(keyword) || s.SiteAddress.Contains(keyword));

                    if (countyId.HasValue)
                        wqQuery = wqQuery.Where(s => s.CountyId == countyId.Value);

                    if (townId.HasValue)
                        wqQuery = wqQuery.Where(s => s.TownId == townId.Value);

                    var wqStations = await wqQuery
                        .Select(s => new
                        {
                            Type = "WQ",
                            TypeName = "水質測站",
                            s.StId,
                            StationId = s.SiteId,
                            StationName = s.SiteName,
                            s.SiteAddress,
                            s.TWD97Lat,
                            s.TWD97Lon,
                            CountyName = s.County.CountyName,
                            TownName = s.Town.TownName
                        })
                        .ToListAsync();

                    results.AddRange(wqStations);
                }

                // 搜尋地下水測站
                if (string.IsNullOrEmpty(stationType) || stationType.ToUpper() == "UG")
                {
                    var ugQuery = _context.UG_Sts
                        .Include(s => s.County)
                        .Include(s => s.Town)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(keyword))
                        ugQuery = ugQuery.Where(s => s.SiteName.Contains(keyword) || s.SiteAddress.Contains(keyword));

                    if (countyId.HasValue)
                        ugQuery = ugQuery.Where(s => s.CountyId == countyId.Value);

                    if (townId.HasValue)
                        ugQuery = ugQuery.Where(s => s.TownId == townId.Value);

                    var ugStations = await ugQuery
                        .Select(s => new
                        {
                            Type = "UG",
                            TypeName = "地下水測站",
                            s.StId,
                            StationId = s.SiteId,
                            StationName = s.SiteName,
                            s.SiteAddress,
                            s.TWD97Lat,
                            s.TWD97Lon,
                            CountyName = s.County.CountyName,
                            TownName = s.Town.TownName
                        })
                        .ToListAsync();

                    results.AddRange(ugStations);
                }

                // 搜尋灌溉水質測站
                if (string.IsNullOrEmpty(stationType) || stationType.ToUpper() == "IWQ")
                {
                    var iwqQuery = _context.IWQ_Sts
                        .Include(s => s.County)
                        .Include(s => s.Town)
                        .Include(s => s.Dept)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(keyword))
                        iwqQuery = iwqQuery.Where(s => s.StName.Contains(keyword) || s.SiteAddress.Contains(keyword));

                    if (countyId.HasValue)
                        iwqQuery = iwqQuery.Where(s => s.CountyId == countyId.Value);

                    if (townId.HasValue)
                        iwqQuery = iwqQuery.Where(s => s.TownId == townId.Value);

                    var iwqStations = await iwqQuery
                        .Select(s => new
                        {
                            Type = "IWQ",
                            TypeName = "灌溉水質測站",
                            s.StId,
                            StationId = (string)null,
                            StationName = s.StName,
                            s.SiteAddress,
                            s.TWD97Lat,
                            s.TWD97Lon,
                            CountyName = s.County.CountyName,
                            TownName = s.Town.TownName,
                            DeptName = s.Dept.DeptName
                        })
                        .ToListAsync();

                    results.AddRange(iwqStations);
                }

                return Ok(new { data = results, count = results.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "搜尋測站時發生錯誤", error = ex.Message });
            }
        }

        /// <summary>
        /// 搜尋附近測站
        /// </summary>
        /// <param name="latitude">緯度</param>
        /// <param name="longitude">經度</param>
        /// <param name="radiusKm">搜尋半徑（公里，預設10公里）</param>
        /// <param name="stationType">測站類型（可選）：WQ-水質、UG-地下水、IWQ-灌溉水質</param>
        /// <returns>附近測站清單（依距離排序）</returns>
        /// <response code="200">搜尋成功</response>
        /// <response code="500">伺服器內部錯誤</response>
        [HttpGet("nearby-stations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNearbyStations(
            [FromQuery] decimal latitude,
            [FromQuery] decimal longitude,
            [FromQuery] double radiusKm = 10.0,
            [FromQuery] string? stationType = null)
        {
            try
            {
                var results = new List<object>();

                // 搜尋附近的水質測站
                if (string.IsNullOrEmpty(stationType) || stationType.ToUpper() == "WQ")
                {
                    var wqStations = await _context.WQ_Sts
                        .Include(s => s.County)
                        .Include(s => s.Town)
                        .Where(s => s.TWD97Lat.HasValue && s.TWD97Lon.HasValue)
                        .ToListAsync();

                    var nearbyWQ = wqStations
                        .Where(s => CalculateDistance((double)latitude, (double)longitude, 
                                                    (double)s.TWD97Lat.Value, (double)s.TWD97Lon.Value) <= radiusKm)
                        .Select(s => new
                        {
                            Type = "WQ",
                            TypeName = "水質測站",
                            s.StId,
                            StationId = s.SiteId,
                            StationName = s.SiteName,
                            s.SiteAddress,
                            s.TWD97Lat,
                            s.TWD97Lon,
                            CountyName = s.County?.CountyName,
                            TownName = s.Town?.TownName,
                            Distance = Math.Round(CalculateDistance((double)latitude, (double)longitude, 
                                                                  (double)s.TWD97Lat.Value, (double)s.TWD97Lon.Value), 2)
                        })
                        .OrderBy(s => s.Distance);

                    results.AddRange(nearbyWQ);
                }

                // 搜尋附近的地下水測站
                if (string.IsNullOrEmpty(stationType) || stationType.ToUpper() == "UG")
                {
                    var ugStations = await _context.UG_Sts
                        .Include(s => s.County)
                        .Include(s => s.Town)
                        .Where(s => s.TWD97Lat.HasValue && s.TWD97Lon.HasValue)
                        .ToListAsync();

                    var nearbyUG = ugStations
                        .Where(s => CalculateDistance((double)latitude, (double)longitude, 
                                                    (double)s.TWD97Lat.Value, (double)s.TWD97Lon.Value) <= radiusKm)
                        .Select(s => new
                        {
                            Type = "UG",
                            TypeName = "地下水測站",
                            s.StId,
                            StationId = s.SiteId,
                            StationName = s.SiteName,
                            s.SiteAddress,
                            s.TWD97Lat,
                            s.TWD97Lon,
                            CountyName = s.County?.CountyName,
                            TownName = s.Town?.TownName,
                            Distance = Math.Round(CalculateDistance((double)latitude, (double)longitude, 
                                                                  (double)s.TWD97Lat.Value, (double)s.TWD97Lon.Value), 2)
                        })
                        .OrderBy(s => s.Distance);

                    results.AddRange(nearbyUG);
                }

                // 搜尋附近的灌溉水質測站
                if (string.IsNullOrEmpty(stationType) || stationType.ToUpper() == "IWQ")
                {
                    var iwqStations = await _context.IWQ_Sts
                        .Include(s => s.County)
                        .Include(s => s.Town)
                        .Include(s => s.Dept)
                        .Where(s => s.TWD97Lat.HasValue && s.TWD97Lon.HasValue)
                        .ToListAsync();

                    var nearbyIWQ = iwqStations
                        .Where(s => CalculateDistance((double)latitude, (double)longitude, 
                                                    (double)s.TWD97Lat.Value, (double)s.TWD97Lon.Value) <= radiusKm)
                        .Select(s => new
                        {
                            Type = "IWQ",
                            TypeName = "灌溉水質測站",
                            s.StId,
                            StationId = (string)null,
                            StationName = s.StName,
                            s.SiteAddress,
                            s.TWD97Lat,
                            s.TWD97Lon,
                            CountyName = s.County?.CountyName,
                            TownName = s.Town?.TownName,
                            DeptName = s.Dept?.DeptName,
                            Distance = Math.Round(CalculateDistance((double)latitude, (double)longitude, 
                                                                  (double)s.TWD97Lat.Value, (double)s.TWD97Lon.Value), 2)
                        })
                        .OrderBy(s => s.Distance);

                    results.AddRange(nearbyIWQ);
                }

                return Ok(new { data = results.OrderBy(r => ((dynamic)r).Distance), count = results.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "搜尋附近測站時發生錯誤", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得最近的監測資料
        /// </summary>
        /// <param name="dataType">資料類型：WQ-水質、UG-地下水、IWQ-灌溉水質（預設WQ）</param>
        /// <param name="days">天數（預設7天）</param>
        /// <param name="limit">筆數限制（預設50筆）</param>
        /// <returns>最近的監測資料</returns>
        /// <response code="200">取得成功</response>
        /// <response code="500">伺服器內部錯誤</response>
        [HttpGet("recent-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRecentData(
            [FromQuery] string dataType = "WQ", // "WQ", "UG", "IWQ"
            [FromQuery] int days = 7,
            [FromQuery] int limit = 50)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-days);
                var results = new List<object>();

                switch (dataType.ToUpper())
                {
                    case "WQ":
                        var wqRecords = await _context.WQ_Records
                            .Include(r => r.St)
                            .Include(r => r.Item)
                            .Where(r => r.SampleDate >= cutoffDate)
                            .OrderByDescending(r => r.SampleDate)
                            .Take(limit)
                            .Select(r => new
                            {
                                Type = "WQ",
                                r.RecordId,
                                r.SampleDate,
                                r.ItemValue,
                                r.ItemValue_Num,
                                StationName = r.St.SiteName,
                                ItemName = r.Item.ItemName,
                                ItemUnit = r.Item.ItemUnit
                            })
                            .ToListAsync();
                        results.AddRange(wqRecords);
                        break;

                    case "UG":
                        var ugRecords = await _context.UG_Records
                            .Include(r => r.St)
                            .Include(r => r.Item)
                            .Where(r => r.SampleDate >= cutoffDate)
                            .OrderByDescending(r => r.SampleDate)
                            .Take(limit)
                            .Select(r => new
                            {
                                Type = "UG",
                                r.RecordId,
                                r.SampleDate,
                                r.ItemValue,
                                r.ItemValue_Num,
                                StationName = r.St.SiteName,
                                ItemName = r.Item.ItemName,
                                ItemUnit = r.Item.ItemUnit
                            })
                            .ToListAsync();
                        results.AddRange(ugRecords);
                        break;

                    case "IWQ":
                        var iwqRecords = await _context.IWQ_Records
                            .Include(r => r.MP)
                            .ThenInclude(mp => mp.St)
                            .Where(r => r.SampleDate >= cutoffDate)
                            .OrderByDescending(r => r.SampleDate)
                            .Take(limit)
                            .Select(r => new
                            {
                                Type = "IWQ",
                                r.RecordId,
                                r.SampleDate,
                                r.PH,
                                r.Temp,
                                r.EC,
                                StationName = r.MP.St.StName,
                                MonitoringPointName = r.MP.MPName
                            })
                            .ToListAsync();
                        results.AddRange(iwqRecords);
                        break;
                }

                return Ok(new { data = results, count = results.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得最近資料時發生錯誤", error = ex.Message });
            }
        }

        /// <summary>
        /// 計算兩個座標之間的距離（公里）
        /// </summary>
        /// <param name="lat1">起點緯度</param>
        /// <param name="lon1">起點經度</param>
        /// <param name="lat2">終點緯度</param>
        /// <param name="lon2">終點經度</param>
        /// <returns>距離（公里）</returns>
        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // 地球半徑（公里）
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        /// <summary>
        /// 將角度轉換為弧度
        /// </summary>
        /// <param name="degrees">角度</param>
        /// <returns>弧度</returns>
        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
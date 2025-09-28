using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgHack.Models;

namespace AgHack.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchApiController : ControllerBase
    {
        private readonly AgHackContext _context;

        public SearchApiController(AgHackContext context)
        {
            _context = context;
        }

        // GET: api/SearchApi/stations
        [HttpGet("stations")]
        public async Task<IActionResult> SearchStations(
            [FromQuery] string? keyword = null,
            [FromQuery] int? countyId = null,
            [FromQuery] int? townId = null,
            [FromQuery] string? stationType = null) // "WQ", "UG", "IWQ"
        {
            try
            {
                var results = new List<object>();

                // 穓碝借代
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
                            TypeName = "借代",
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

                // 穓碝代
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
                            TypeName = "代",
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

                // 穓碝穨紀代
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
                            TypeName = "穨紀代",
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
                return StatusCode(500, new { message = "穓碝代祇ネ岿粇", error = ex.Message });
            }
        }

        // GET: api/SearchApi/nearby-stations
        [HttpGet("nearby-stations")]
        public async Task<IActionResult> GetNearbyStations(
            [FromQuery] decimal latitude,
            [FromQuery] decimal longitude,
            [FromQuery] double radiusKm = 10.0,
            [FromQuery] string? stationType = null)
        {
            try
            {
                var results = new List<object>();

                // 穓碝借代
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
                            TypeName = "借代",
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

                // 穓碝代
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
                            TypeName = "代",
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

                return Ok(new { data = results.OrderBy(r => ((dynamic)r).Distance), count = results.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "穓碝代祇ネ岿粇", error = ex.Message });
            }
        }

        // GET: api/SearchApi/recent-data
        [HttpGet("recent-data")]
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
                return StatusCode(500, new { message = "眔程戈祇ネ岿粇", error = ex.Message });
            }
        }

        // 璸衡ㄢ畒夹ぇ丁禯瞒 (そń)
        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // 瞴畖 (そń)
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
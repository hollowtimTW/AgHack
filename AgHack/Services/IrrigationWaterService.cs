using Microsoft.EntityFrameworkCore;
using AgHack.Models;
using AgHack.Models.DTOs;

namespace AgHack.Services
{
    /// <summary>
    /// 灌溉用水服務類別
    /// </summary>
    public class IrrigationWaterService : IIrrigationWaterService
    {
        private readonly AgHackContext _context;

        public IrrigationWaterService(AgHackContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 取得部門清單
        /// </summary>
        public async Task<ApiResponse<List<object>>> GetDepartmentsAsync()
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

                var deptList = departments.Cast<object>().ToList();
                return ApiResponse<List<object>>.SuccessResult(deptList, "取得部門資料成功");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<object>>.ErrorResult(
                    "取得部門資料時發生錯誤", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// 取得灌溉用水測站清單
        /// </summary>
        public async Task<ApiResponse<List<IrrigationWaterStationDto>>> GetStationsAsync(int? deptId = null)
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
                    .Select(s => new IrrigationWaterStationDto
                    {
                        StId = s.StId,
                        StName = s.StName,
                        SiteName = s.StName,
                        SiteAddress = s.SiteAddress,
                        TWD97Lat = s.TWD97Lat,
                        TWD97Lon = s.TWD97Lon,
                        CountyName = s.County.CountyName,
                        TownName = s.Town.TownName,
                        DeptName = s.Dept.DeptName,
                        StationType = "IWQ"
                    })
                    .ToListAsync();

                return ApiResponse<List<IrrigationWaterStationDto>>.SuccessResult(stations, "取得灌溉用水測站成功");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<IrrigationWaterStationDto>>.ErrorResult(
                    "取得灌溉用水測站時發生錯誤", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// 取得特定灌溉用水測站
        /// </summary>
        public async Task<ApiResponse<IrrigationWaterStationDto>> GetStationAsync(int id)
        {
            try
            {
                var station = await _context.IWQ_Sts
                    .Include(s => s.County)
                    .Include(s => s.Town)
                    .Include(s => s.Dept)
                    .Where(s => s.StId == id)
                    .Select(s => new IrrigationWaterStationDto
                    {
                        StId = s.StId,
                        StName = s.StName,
                        SiteName = s.StName,
                        SiteAddress = s.SiteAddress,
                        TWD97Lat = s.TWD97Lat,
                        TWD97Lon = s.TWD97Lon,
                        CountyName = s.County.CountyName,
                        TownName = s.Town.TownName,
                        DeptName = s.Dept.DeptName,
                        StationType = "IWQ"
                    })
                    .FirstOrDefaultAsync();

                if (station == null)
                    return ApiResponse<IrrigationWaterStationDto>.NotFoundResult("找不到指定的灌溉用水測站");

                return ApiResponse<IrrigationWaterStationDto>.SuccessResult(station, "取得灌溉用水測站詳情成功");
            }
            catch (Exception ex)
            {
                return ApiResponse<IrrigationWaterStationDto>.ErrorResult(
                    "取得灌溉用水測站時發生錯誤", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// 取得監測點清單
        /// </summary>
        public async Task<ApiResponse<List<object>>> GetMonitoringPointsAsync(int? stationId = null, int? deptId = null)
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

                var mpList = monitoringPoints.Cast<object>().ToList();
                return ApiResponse<List<object>>.SuccessResult(mpList, "取得監測點資料成功");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<object>>.ErrorResult(
                    "取得監測點資料時發生錯誤", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// 取得灌溉用水監測記錄
        /// </summary>
        public async Task<ApiResponse<List<IrrigationWaterRecordDto>>> GetRecordsAsync(
            int? mpId = null, 
            DateTime? startDate = null, 
            DateTime? endDate = null, 
            int page = 1, 
            int pageSize = 100)
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
                    .Select(r => new IrrigationWaterRecordDto
                    {
                        RecordId = r.RecordId,
                        SampleDate = r.SampleDate,
                        PH = r.PH,
                        Temp = r.Temp,
                        EC = r.EC,
                        Note = r.Note,
                        MonitoringPointName = r.MP.MPName,
                        StationName = r.MP.St.StName
                    })
                    .ToListAsync();

                var pagination = PaginationInfo.Create(page, pageSize, totalCount);
                return ApiResponse<List<IrrigationWaterRecordDto>>.SuccessResult(records, totalCount, pagination, "取得灌溉用水監測資料成功");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<IrrigationWaterRecordDto>>.ErrorResult(
                    "取得灌溉用水監測資料時發生錯誤", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// 取得灌溉用水統計資料
        /// </summary>
        public async Task<ApiResponse<IrrigationWaterStatisticsDto>> GetStatisticsAsync(
            int? mpId = null, 
            DateTime? startDate = null, 
            DateTime? endDate = null)
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
                    .Select(g => new IrrigationWaterStatisticsDto
                    {
                        Count = g.Count(),
                        PH = new StatisticsDto
                        {
                            Count = g.Count(r => r.PH.HasValue),
                            Average = g.Where(r => r.PH.HasValue).Average(r => r.PH),
                            Min = g.Where(r => r.PH.HasValue).Min(r => r.PH),
                            Max = g.Where(r => r.PH.HasValue).Max(r => r.PH)
                        },
                        Temperature = new StatisticsDto
                        {
                            Count = g.Count(r => r.Temp.HasValue),
                            Average = g.Where(r => r.Temp.HasValue).Average(r => r.Temp),
                            Min = g.Where(r => r.Temp.HasValue).Min(r => r.Temp),
                            Max = g.Where(r => r.Temp.HasValue).Max(r => r.Temp)
                        },
                        EC = new StatisticsDto
                        {
                            Count = g.Count(r => r.EC.HasValue),
                            Average = g.Where(r => r.EC.HasValue).Average(r => r.EC),
                            Min = g.Where(r => r.EC.HasValue).Min(r => r.EC),
                            Max = g.Where(r => r.EC.HasValue).Max(r => r.EC)
                        }
                    })
                    .FirstOrDefaultAsync();

                var result = statistics ?? new IrrigationWaterStatisticsDto
                {
                    Count = 0,
                    PH = new StatisticsDto { Count = 0, Average = null, Min = null, Max = null },
                    Temperature = new StatisticsDto { Count = 0, Average = null, Min = null, Max = null },
                    EC = new StatisticsDto { Count = 0, Average = null, Min = null, Max = null }
                };

                return ApiResponse<IrrigationWaterStatisticsDto>.SuccessResult(result, "取得灌溉用水統計資料成功");
            }
            catch (Exception ex)
            {
                return ApiResponse<IrrigationWaterStatisticsDto>.ErrorResult(
                    "取得灌溉用水統計資料時發生錯誤", new List<string> { ex.Message });
            }
        }
    }
}
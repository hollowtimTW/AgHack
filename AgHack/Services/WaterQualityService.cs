using Microsoft.EntityFrameworkCore;
using AgHack.Models;
using AgHack.Models.DTOs;

namespace AgHack.Services
{
    /// <summary>
    /// 水質監測服務實作
    /// </summary>
    public class WaterQualityService : IWaterQualityService
    {
        private readonly IGenericRepository<WQ_St> _stationRepository;
        private readonly IGenericRepository<WQ_Record> _recordRepository;
        private readonly IGenericRepository<WQ_Item> _itemRepository;
        private readonly AgHackContext _context;

        public WaterQualityService(
            IGenericRepository<WQ_St> stationRepository,
            IGenericRepository<WQ_Record> recordRepository,
            IGenericRepository<WQ_Item> itemRepository,
            AgHackContext context)
        {
            _stationRepository = stationRepository;
            _recordRepository = recordRepository;
            _itemRepository = itemRepository;
            _context = context;
        }

        /// <summary>
        /// 取得所有水質測站
        /// </summary>
        public async Task<ApiResponse<List<WaterQualityStationDto>>> GetStationsAsync()
        {
            try
            {
                var stations = await _context.WQ_Sts
                    .Include(s => s.County)
                    .Include(s => s.Town)
                    .Include(s => s.Basin)
                    .Select(s => new WaterQualityStationDto
                    {
                        StId = s.StId,
                        SiteId = s.SiteId,
                        SiteName = s.SiteName,
                        SiteEngName = s.SiteEngName,
                        SiteAddress = s.SiteAddress,
                        TWD97Lat = s.TWD97Lat,
                        TWD97Lon = s.TWD97Lon,
                        River = s.River,
                        CountyName = s.County.CountyName,
                        TownName = s.Town.TownName,
                        BasinName = s.Basin.BasinName,
                        StationType = "WQ"
                    })
                    .ToListAsync();

                return ApiResponse<List<WaterQualityStationDto>>.SuccessResult(stations, "取得水質測站成功");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WaterQualityStationDto>>.ErrorResult(
                    "取得水質測站時發生錯誤", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// 取得特定水質測站
        /// </summary>
        public async Task<ApiResponse<WaterQualityStationDto>> GetStationAsync(int id)
        {
            try
            {
                var station = await _context.WQ_Sts
                    .Include(s => s.County)
                    .Include(s => s.Town)
                    .Include(s => s.Basin)
                    .Where(s => s.StId == id)
                    .Select(s => new WaterQualityStationDto
                    {
                        StId = s.StId,
                        SiteId = s.SiteId,
                        SiteName = s.SiteName,
                        SiteEngName = s.SiteEngName,
                        SiteAddress = s.SiteAddress,
                        TWD97Lat = s.TWD97Lat,
                        TWD97Lon = s.TWD97Lon,
                        River = s.River,
                        CountyName = s.County.CountyName,
                        TownName = s.Town.TownName,
                        BasinName = s.Basin.BasinName,
                        StationType = "WQ"
                    })
                    .FirstOrDefaultAsync();

                if (station == null)
                    return ApiResponse<WaterQualityStationDto>.NotFoundResult("找不到指定的水質測站");

                return ApiResponse<WaterQualityStationDto>.SuccessResult(station, "取得水質測站詳情成功");
            }
            catch (Exception ex)
            {
                return ApiResponse<WaterQualityStationDto>.ErrorResult(
                    "取得水質測站時發生錯誤", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// 取得水質監測記錄
        /// </summary>
        public async Task<ApiResponse<List<RecordListDto>>> GetRecordsAsync(RecordSearchDto searchDto)
        {
            try
            {
                var query = _context.WQ_Records
                    .Include(r => r.St)
                    .Include(r => r.Item)
                    .ThenInclude(i => i.ItemCategory)
                    .AsQueryable();

                if (searchDto.StationId.HasValue)
                    query = query.Where(r => r.StId == searchDto.StationId.Value);

                if (searchDto.ItemId.HasValue)
                    query = query.Where(r => r.ItemId == searchDto.ItemId.Value);

                if (searchDto.StartDate.HasValue)
                    query = query.Where(r => r.SampleDate >= searchDto.StartDate.Value);

                if (searchDto.EndDate.HasValue)
                    query = query.Where(r => r.SampleDate <= searchDto.EndDate.Value);

                var totalCount = await query.CountAsync();
                var records = await query
                    .OrderByDescending(r => r.SampleDate)
                    .Skip((searchDto.Page - 1) * searchDto.PageSize)
                    .Take(searchDto.PageSize)
                    .Select(r => new RecordListDto
                    {
                        RecordId = r.RecordId,
                        SampleDate = r.SampleDate,
                        ItemValue = r.ItemValue,
                        ItemValue_Num = r.ItemValue_Num,
                        Note = r.Note,
                        StationName = r.St.SiteName,
                        StationId = r.St.StId,
                        ItemName = r.Item.ItemName,
                        ItemUnit = r.Item.ItemUnit,
                        CategoryName = r.Item.ItemCategory.ItemCategoryName
                    })
                    .ToListAsync();

                var pagination = PaginationInfo.Create(searchDto.Page, searchDto.PageSize, totalCount);
                return ApiResponse<List<RecordListDto>>.SuccessResult(records, totalCount, pagination, "取得水質監測資料成功");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<RecordListDto>>.ErrorResult(
                    "取得監測資料時發生錯誤", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// 取得水質監測項目
        /// </summary>
        public async Task<ApiResponse<List<object>>> GetItemsAsync()
        {
            try
            {
                var items = await _context.WQ_Items
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

                var itemList = items.Cast<object>().ToList();
                return ApiResponse<List<object>>.SuccessResult(itemList, "取得水質監測項目成功");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<object>>.ErrorResult(
                    "取得監測項目時發生錯誤", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// 取得水質統計資料
        /// </summary>
        public async Task<ApiResponse<StatisticsDto>> GetStatisticsAsync(RecordSearchDto searchDto)
        {
            try
            {
                var query = _context.WQ_Records.AsQueryable();

                if (searchDto.StationId.HasValue)
                    query = query.Where(r => r.StId == searchDto.StationId.Value);

                if (searchDto.ItemId.HasValue)
                    query = query.Where(r => r.ItemId == searchDto.ItemId.Value);

                if (searchDto.StartDate.HasValue)
                    query = query.Where(r => r.SampleDate >= searchDto.StartDate.Value);

                if (searchDto.EndDate.HasValue)
                    query = query.Where(r => r.SampleDate <= searchDto.EndDate.Value);

                var numericRecords = query.Where(r => r.ItemValue_Num.HasValue);

                var statistics = await numericRecords
                    .GroupBy(r => 1)
                    .Select(g => new StatisticsDto
                    {
                        Count = g.Count(),
                        Average = g.Average(r => r.ItemValue_Num),
                        Min = g.Min(r => r.ItemValue_Num),
                        Max = g.Max(r => r.ItemValue_Num)
                    })
                    .FirstOrDefaultAsync();

                var result = statistics ?? new StatisticsDto { Count = 0, Average = null, Min = null, Max = null };
                return ApiResponse<StatisticsDto>.SuccessResult(result, "取得統計資料成功");
            }
            catch (Exception ex)
            {
                return ApiResponse<StatisticsDto>.ErrorResult(
                    "取得統計資料時發生錯誤", new List<string> { ex.Message });
            }
        }
    }
}
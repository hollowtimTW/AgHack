using AgHack.Models.DTOs;
using AgHack.Models;

namespace AgHack.Services
{
    /// <summary>
    /// 水質監測服務介面
    /// </summary>
    public interface IWaterQualityService
    {
        /// <summary>
        /// 取得所有水質測站
        /// </summary>
        Task<ApiResponse<List<WaterQualityStationDto>>> GetStationsAsync();
        
        /// <summary>
        /// 取得特定水質測站
        /// </summary>
        /// <param name="id">測站ID</param>
        Task<ApiResponse<WaterQualityStationDto>> GetStationAsync(int id);
        
        /// <summary>
        /// 取得水質監測記錄
        /// </summary>
        /// <param name="searchDto">搜尋條件</param>
        Task<ApiResponse<List<RecordListDto>>> GetRecordsAsync(RecordSearchDto searchDto);
        
        /// <summary>
        /// 取得水質監測項目
        /// </summary>
        Task<ApiResponse<List<object>>> GetItemsAsync();
        
        /// <summary>
        /// 取得水質統計資料
        /// </summary>
        /// <param name="searchDto">搜尋條件</param>
        Task<ApiResponse<StatisticsDto>> GetStatisticsAsync(RecordSearchDto searchDto);
    }
}
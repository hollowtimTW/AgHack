using AgHack.Models.DTOs;

namespace AgHack.Services
{
    /// <summary>
    /// 灌溉用水服務介面
    /// </summary>
    public interface IIrrigationWaterService
    {
        /// <summary>
        /// 取得部門清單
        /// </summary>
        Task<ApiResponse<List<object>>> GetDepartmentsAsync();
        
        /// <summary>
        /// 取得灌溉用水測站清單
        /// </summary>
        /// <param name="deptId">部門ID（可選）</param>
        Task<ApiResponse<List<IrrigationWaterStationDto>>> GetStationsAsync(int? deptId = null);
        
        /// <summary>
        /// 取得特定灌溉用水測站
        /// </summary>
        /// <param name="id">測站ID</param>
        Task<ApiResponse<IrrigationWaterStationDto>> GetStationAsync(int id);
        
        /// <summary>
        /// 取得監測點清單
        /// </summary>
        /// <param name="stationId">測站ID（可選）</param>
        /// <param name="deptId">部門ID（可選）</param>
        Task<ApiResponse<List<object>>> GetMonitoringPointsAsync(int? stationId = null, int? deptId = null);
        
        /// <summary>
        /// 取得灌溉用水監測記錄
        /// </summary>
        /// <param name="mpId">監測點ID（可選）</param>
        /// <param name="startDate">開始日期（可選）</param>
        /// <param name="endDate">結束日期（可選）</param>
        /// <param name="page">頁碼</param>
        /// <param name="pageSize">每頁筆數</param>
        Task<ApiResponse<List<IrrigationWaterRecordDto>>> GetRecordsAsync(int? mpId = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 100);
        
        /// <summary>
        /// 取得灌溉用水統計資料
        /// </summary>
        /// <param name="mpId">監測點ID（可選）</param>
        /// <param name="startDate">開始日期（可選）</param>
        /// <param name="endDate">結束日期（可選）</param>
        Task<ApiResponse<IrrigationWaterStatisticsDto>> GetStatisticsAsync(int? mpId = null, DateTime? startDate = null, DateTime? endDate = null);
    }
}
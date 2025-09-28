using AgHack.Models.DTOs;
using AgHack.Models;

namespace AgHack.Services
{
    /// <summary>
    /// ����ʴ��A�Ȥ���
    /// </summary>
    public interface IWaterQualityService
    {
        /// <summary>
        /// ���o�Ҧ��������
        /// </summary>
        Task<ApiResponse<List<WaterQualityStationDto>>> GetStationsAsync();
        
        /// <summary>
        /// ���o�S�w�������
        /// </summary>
        /// <param name="id">����ID</param>
        Task<ApiResponse<WaterQualityStationDto>> GetStationAsync(int id);
        
        /// <summary>
        /// ���o����ʴ��O��
        /// </summary>
        /// <param name="searchDto">�j�M����</param>
        Task<ApiResponse<List<RecordListDto>>> GetRecordsAsync(RecordSearchDto searchDto);
        
        /// <summary>
        /// ���o����ʴ�����
        /// </summary>
        Task<ApiResponse<List<object>>> GetItemsAsync();
        
        /// <summary>
        /// ���o����έp���
        /// </summary>
        /// <param name="searchDto">�j�M����</param>
        Task<ApiResponse<StatisticsDto>> GetStatisticsAsync(RecordSearchDto searchDto);
    }
}
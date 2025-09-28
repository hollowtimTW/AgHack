using AgHack.Models.DTOs;

namespace AgHack.Services
{
    /// <summary>
    /// ��@�Τ��A�Ȥ���
    /// </summary>
    public interface IIrrigationWaterService
    {
        /// <summary>
        /// ���o�����M��
        /// </summary>
        Task<ApiResponse<List<object>>> GetDepartmentsAsync();
        
        /// <summary>
        /// ���o��@�Τ������M��
        /// </summary>
        /// <param name="deptId">����ID�]�i��^</param>
        Task<ApiResponse<List<IrrigationWaterStationDto>>> GetStationsAsync(int? deptId = null);
        
        /// <summary>
        /// ���o�S�w��@�Τ�����
        /// </summary>
        /// <param name="id">����ID</param>
        Task<ApiResponse<IrrigationWaterStationDto>> GetStationAsync(int id);
        
        /// <summary>
        /// ���o�ʴ��I�M��
        /// </summary>
        /// <param name="stationId">����ID�]�i��^</param>
        /// <param name="deptId">����ID�]�i��^</param>
        Task<ApiResponse<List<object>>> GetMonitoringPointsAsync(int? stationId = null, int? deptId = null);
        
        /// <summary>
        /// ���o��@�Τ��ʴ��O��
        /// </summary>
        /// <param name="mpId">�ʴ��IID�]�i��^</param>
        /// <param name="startDate">�}�l����]�i��^</param>
        /// <param name="endDate">��������]�i��^</param>
        /// <param name="page">���X</param>
        /// <param name="pageSize">�C������</param>
        Task<ApiResponse<List<IrrigationWaterRecordDto>>> GetRecordsAsync(int? mpId = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 100);
        
        /// <summary>
        /// ���o��@�Τ��έp���
        /// </summary>
        /// <param name="mpId">�ʴ��IID�]�i��^</param>
        /// <param name="startDate">�}�l����]�i��^</param>
        /// <param name="endDate">��������]�i��^</param>
        Task<ApiResponse<IrrigationWaterStatisticsDto>> GetStatisticsAsync(int? mpId = null, DateTime? startDate = null, DateTime? endDate = null);
    }
}
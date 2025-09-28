namespace AgHack.Models.DTOs
{
    public class RecordListDto
    {
        public int RecordId { get; set; }
        public DateTime SampleDate { get; set; }
        public string? ItemValue { get; set; }
        public decimal? ItemValue_Num { get; set; }
        public string? Note { get; set; }
        public string StationName { get; set; } = string.Empty;
        public int StationId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? ItemUnit { get; set; }
        public string? CategoryName { get; set; }
    }

    public class IrrigationWaterRecordDto
    {
        public int RecordId { get; set; }
        public DateTime? SampleDate { get; set; }
        public decimal? PH { get; set; }
        public decimal? Temp { get; set; }
        public decimal? EC { get; set; }
        public string? Note { get; set; }
        public string MonitoringPointName { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
    }

    public class RecordSearchDto
    {
        public int? StationId { get; set; }
        public int? ItemId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }

    public class StatisticsDto
    {
        public int Count { get; set; }
        public decimal? Average { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }

    public class IrrigationWaterStatisticsDto
    {
        public int Count { get; set; }
        public StatisticsDto PH { get; set; } = new();
        public StatisticsDto Temperature { get; set; } = new();
        public StatisticsDto EC { get; set; } = new();
    }
}
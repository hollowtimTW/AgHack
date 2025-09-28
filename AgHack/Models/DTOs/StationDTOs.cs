namespace AgHack.Models.DTOs
{
    public class StationListDto
    {
        public int StId { get; set; }
        public string? SiteId { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public string? SiteEngName { get; set; }
        public string? SiteAddress { get; set; }
        public decimal? TWD97Lat { get; set; }
        public decimal? TWD97Lon { get; set; }
        public string? CountyName { get; set; }
        public string? TownName { get; set; }
        public string StationType { get; set; } = string.Empty;
    }

    public class StationDetailDto : StationListDto
    {
        public decimal? TWD97TM2X { get; set; }
        public decimal? TWD97TM2Y { get; set; }
        public string? AdditionalInfo { get; set; }
    }

    public class WaterQualityStationDto : StationDetailDto
    {
        public string? River { get; set; }
        public string? BasinName { get; set; }
    }

    public class GroundwaterStationDto : StationDetailDto
    {
        public string? UGWDistName { get; set; }
    }

    public class IrrigationWaterStationDto : StationDetailDto
    {
        public string? StName { get; set; }
        public string? DeptName { get; set; }
    }

    public class StationSearchDto
    {
        public string? Keyword { get; set; }
        public int? CountyId { get; set; }
        public int? TownId { get; set; }
        public string? StationType { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class NearbyStationSearchDto
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public double RadiusKm { get; set; } = 10.0;
        public string? StationType { get; set; }
    }
}
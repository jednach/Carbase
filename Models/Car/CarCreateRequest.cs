namespace Carbase.Models.Car
{
    public class CarCreateRequest
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public string? Tuner { get; set; }

        public int? WeightKg { get; set; }
        public int? HorsePower { get; set; }
        public int? TorqueNm { get; set; }
        public int? TopSpeed { get; set; }

        public decimal? Time0To100 { get; set; }
        public decimal? Time100To200 { get; set; }
        public decimal? Time200To250 { get; set; }
        public decimal? TimeQuarterMile { get; set; }
    }
}

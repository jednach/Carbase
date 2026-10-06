namespace Carbase.Models.Car
{
    public class CarEditRequest
    {
        public long Id { get; set; }

        public IFormFile? Image { get; set; }
        public string? ImagePath { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
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

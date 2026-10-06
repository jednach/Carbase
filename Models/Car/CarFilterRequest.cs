using Carbase.Models.Filters;

namespace Carbase.Models.Car
{
    public class CarFilterRequest
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }

        public NumericFilter<int> Year { get; set; } = new();

        public PresenceFilter TunerStatus { get; set; } = PresenceFilter.All;
        public string? Tuner { get; set; }

        public NumericFilter<int> WeightKg { get; set; } = new();
        public NumericFilter<int> HorsePower { get; set; } = new();
        public NumericFilter<int> TorqueNm { get; set; } = new();
        public NumericFilter<int> TopSpeed { get; set; } = new();

        public NumericFilter<decimal> Time0To100 { get; set; } = new();
        public NumericFilter<decimal> Time100To200 { get; set; } = new();
        public NumericFilter<decimal> Time200To250 { get; set; } = new();
        public NumericFilter<decimal> TimeQuarterMile { get; set; } = new();
    }
}

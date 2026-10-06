using Carbase.Models.Filters;

namespace Carbase.Models.Car
{
    public class CarFilterRequest
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }

        public PresenceFilter TunerStatus { get; set; } = PresenceFilter.All;
        public string? Tuner { get; set; }
    }
}

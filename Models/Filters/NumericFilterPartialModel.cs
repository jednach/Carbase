namespace Carbase.Models.Filters
{
    public class NumericFilterPartialModel
    {
        public string Label { get; set; } = string.Empty;

        public string Prefix { get; set; } = string.Empty;

        public NumericFilterType Type { get; set; }

        public object? Value { get; set; }
        public object? From { get; set; }
        public object? To { get; set; }

        public bool AllowPresence { get; set; } = true;
    }
}

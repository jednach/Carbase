namespace Carbase.Models.Filters
{
    public class NumericFilter<T> where T : struct
    {
        public NumericFilterType Type { get; set; } = NumericFilterType.All;

        public T? Value { get; set; }

        public T? From { get; set; }

        public T? To { get; set; }
    }
}

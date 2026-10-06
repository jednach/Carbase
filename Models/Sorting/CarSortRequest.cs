namespace Carbase.Models.Sorting
{
    public class CarSortRequest
    {
        public CarSortField Field { get; set; } = CarSortField.None;

        public SortDirection Direction { get; set; } =
            SortDirection.Ascending;
    }
}

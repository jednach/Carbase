using Carbase.Models.Car;
using Carbase.Models.Filters;
using Microsoft.EntityFrameworkCore;

namespace Carbase.Services
{
    public class CarQueryService
    {
        public IQueryable<Car> ApplyFilters(
            IQueryable<Car> query,
            CarFilterRequest filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Brand))
            {
                var brand = filter.Brand.Trim();

                query = query.Where(c =>
                    EF.Functions.ILike(c.Brand, $"%{brand}%"));
            }

            if (!string.IsNullOrWhiteSpace(filter.Model))
            {
                var model = filter.Model.Trim();

                query = query.Where(c =>
                    EF.Functions.ILike(c.Model, $"%{model}%"));
            }

            if (filter.Year.HasValue)
            {
                query = query.Where(c =>
                    c.Year == filter.Year.Value);
            }

            switch (filter.TunerStatus)
            {
                case PresenceFilter.With:
                    query = query.Where(c => c.Tuner != null);

                    if (!string.IsNullOrWhiteSpace(filter.Tuner))
                    {
                        var tuner = filter.Tuner.Trim();

                        query = query.Where(c =>
                            EF.Functions.ILike(c.Tuner!, $"%{tuner}%"));
                    }

                    break;

                case PresenceFilter.Without:
                    query = query.Where(c => c.Tuner == null);
                    break;
            }

            return query;
        }
    }
}

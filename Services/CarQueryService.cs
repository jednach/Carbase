using Carbase.Models.Car;
using Carbase.Models.Filters;
using Carbase.Models.Sorting;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

            query = ApplyNumericFilter(
                query,
                filter.Year,
                c => (int?)c.Year);

            query = ApplyNumericFilter(
                query,
                filter.WeightKg,
                c => c.WeightKg);

            query = ApplyNumericFilter(
                query,
                filter.HorsePower,
                c => c.HorsePower);

            query = ApplyNumericFilter(
                query,
                filter.TorqueNm,
                c => c.TorqueNm);

            query = ApplyNumericFilter(
                query,
                filter.TopSpeed,
                c => c.TopSpeed);

            query = ApplyNumericFilter(
                query,
                filter.Time0To100,
                c => c.Time0To100);

            query = ApplyNumericFilter(
                query,
                filter.Time100To200,
                c => c.Time100To200);

            query = ApplyNumericFilter(
                query,
                filter.Time200To250,
                c => c.Time200To250);

            query = ApplyNumericFilter(
                query,
                filter.TimeQuarterMile,
                c => c.TimeQuarterMile);

            return query;
        }

        public IQueryable<Car> ApplySorting(
            IQueryable<Car> query,
            CarSortRequest sort)
        {
            return sort.Field switch
            {
                CarSortField.Year =>
                    ApplyNumericSorting(
                        query,
                        sort.Direction,
                        c => (int?)c.Year),

                CarSortField.WeightKg =>
                    ApplyNumericSorting(
                        query,
                        sort.Direction,
                        c => c.WeightKg),

                CarSortField.HorsePower =>
                    ApplyNumericSorting(
                        query,
                        sort.Direction,
                        c => c.HorsePower),

                CarSortField.TorqueNm =>
                    ApplyNumericSorting(
                        query,
                        sort.Direction,
                        c => c.TorqueNm),

                CarSortField.TopSpeed =>
                    ApplyNumericSorting(
                        query,
                        sort.Direction,
                        c => c.TopSpeed),

                CarSortField.Time0To100 =>
                    ApplyNumericSorting(
                        query,
                        sort.Direction,
                        c => c.Time0To100),

                CarSortField.Time100To200 =>
                    ApplyNumericSorting(
                        query,
                        sort.Direction,
                        c => c.Time100To200),

                CarSortField.Time200To250 =>
                    ApplyNumericSorting(
                        query,
                        sort.Direction,
                        c => c.Time200To250),

                CarSortField.TimeQuarterMile =>
                    ApplyNumericSorting(
                        query,
                        sort.Direction,
                        c => c.TimeQuarterMile),

                _ => query
            };
        }

        private static IQueryable<Car> ApplyNumericFilter<T>(
            IQueryable<Car> query,
            NumericFilter<T> filter,
            Expression<Func<Car, T?>> selector)
            where T : struct
        {
            var parameter = selector.Parameters[0];
            var property = selector.Body;

            Expression? condition = filter.Type switch
            {
                NumericFilterType.All =>
                    null,

                NumericFilterType.With =>
                    Expression.NotEqual(
                        property,
                        Expression.Constant(null, typeof(T?))),

                NumericFilterType.Without =>
                    Expression.Equal(
                        property,
                        Expression.Constant(null, typeof(T?))),

                NumericFilterType.Between
                    when filter.From.HasValue && filter.To.HasValue =>
                    Expression.AndAlso(
                        Expression.GreaterThanOrEqual(
                            property,
                            ToNullableConstant(filter.From.Value)),
                        Expression.LessThanOrEqual(
                            property,
                            ToNullableConstant(filter.To.Value))),

                NumericFilterType.LessThan
                    when filter.Value.HasValue =>
                    Expression.LessThan(
                        property,
                        ToNullableConstant(filter.Value.Value)),

                NumericFilterType.GreaterThan
                    when filter.Value.HasValue =>
                    Expression.GreaterThan(
                        property,
                        ToNullableConstant(filter.Value.Value)),

                _ => null
            };

            if (condition is null)
            {
                return query;
            }

            var predicate =
                Expression.Lambda<Func<Car, bool>>(
                    condition,
                    parameter);

            return query.Where(predicate);
        }

        private static IQueryable<Car> ApplyNumericSorting<T>(
            IQueryable<Car> query,
            SortDirection direction,
            Expression<Func<Car, T?>> selector)
            where T : struct
        {
            var parameter = selector.Parameters[0];

            var hasValueExpression = Expression.NotEqual(
                selector.Body,
                Expression.Constant(null, typeof(T?)));

            var hasValueSelector =
                Expression.Lambda<Func<Car, bool>>(
                    hasValueExpression,
                    parameter);

            var orderedQuery =
                query.OrderByDescending(hasValueSelector);

            return direction switch
            {
                SortDirection.Descending =>
                    orderedQuery.ThenByDescending(selector),

                _ =>
                    orderedQuery.ThenBy(selector)
            };
        }

        private static Expression ToNullableConstant<T>(T value)
            where T : struct
        {
            return Expression.Convert(
                Expression.Constant(value, typeof(T)),
                typeof(T?));
        }
    }
}

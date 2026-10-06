using Carbase.Models.Car;
using FluentValidation;

namespace Carbase.Validation.Car
{
    public class CarEditRequestValidator : AbstractValidator<CarEditRequest>
    {
        public CarEditRequestValidator()
        {
            RuleFor(x => x.WeightKg)
                .InclusiveBetween(1, 10000)
                .When(x => x.WeightKg.HasValue)
                .WithMessage("Weight must be between 1 and 10000 kg.");

            RuleFor(x => x.HorsePower)
                .InclusiveBetween(1, 5000)
                .When(x => x.HorsePower.HasValue)
                .WithMessage("HP must be between 1 and 5000.");

            RuleFor(x => x.TorqueNm)
                .InclusiveBetween(1, 10000)
                .When(x => x.TorqueNm.HasValue)
                .WithMessage("Torque must be between 1 and 10000 Nm.");

            RuleFor(x => x.TopSpeed)
                .InclusiveBetween(1, 1000)
                .When(x => x.TopSpeed.HasValue)
                .WithMessage("Top speed must be between 1 and 1000 km/h.");

            RuleFor(x => x.Time0To100)
                .InclusiveBetween(0.1m, 300m)
                .When(x => x.Time0To100.HasValue)
                .WithMessage("0-100 time must be between 0.1 and 300 seconds.");

            RuleFor(x => x.Time100To200)
                .InclusiveBetween(0.1m, 600m)
                .When(x => x.Time100To200.HasValue)
                .WithMessage("100-200 time must be between 0.1 and 600 seconds.");

            RuleFor(x => x.Time200To250)
                .InclusiveBetween(0.1m, 900m)
                .When(x => x.Time200To250.HasValue)
                .WithMessage("200-250 time must be between 0.1 and 900 seconds.");

            RuleFor(x => x.TimeQuarterMile)
                .InclusiveBetween(0.1m, 300m)
                .When(x => x.TimeQuarterMile.HasValue)
                .WithMessage("1/4 mile time must be between 0.1 and 300 seconds.");
        }
    }
}

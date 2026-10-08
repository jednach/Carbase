using Carbase.Data;
using Carbase.Models.Car;
using Carbase.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Carbase.Pages.Cars
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IValidator<CarCreateRequest> _validator;
        private readonly CarImageService _carImageService;

        public CreateModel(
            AppDbContext context,
            IValidator<CarCreateRequest> validator,
            CarImageService carImageService)
        {
            _context = context;
            _validator = validator;
            _carImageService = carImageService;
        }

        [BindProperty]
        public CarCreateRequest CreateRequest { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!IsMultipartFormData())
            {
                return StatusCode(
                    StatusCodes.Status415UnsupportedMediaType);
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var validationResult =
                await _validator.ValidateAsync(CreateRequest);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(
                        $"CreateRequest.{error.PropertyName}",
                        error.ErrorMessage);
                }

                return Page();
            }

            var brand = CreateRequest.Brand!.Trim();
            var model = CreateRequest.Model!.Trim();
            var year = CreateRequest.Year!.Value;
            var tuner = string.IsNullOrWhiteSpace(CreateRequest.Tuner)
                ? null
                : CreateRequest.Tuner.Trim();

            var exists = await _context.Cars.AnyAsync(c =>
                c.Brand == brand &&
                c.Model == model &&
                c.Year == year &&
                c.Tuner == tuner);

            if (exists)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "This car already exists.");

                return Page();
            }

            string? imagePath = null;

            if (CreateRequest.Image is not null)
            {
                try
                {
                    imagePath = await _carImageService.SaveAsync(
                        CreateRequest.Image);
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(
                        "CreateRequest.Image",
                        ex.Message);

                    return Page();
                }
            }

            var car = new Car
            {
                ImagePath = imagePath,
                Brand = brand,
                Model = model,
                Year = year,
                Tuner = tuner,
                WeightKg = CreateRequest.WeightKg,
                HorsePower = CreateRequest.HorsePower,
                TorqueNm = CreateRequest.TorqueNm,
                TopSpeed = CreateRequest.TopSpeed,
                Time0To100 = CreateRequest.Time0To100,
                Time100To200 = CreateRequest.Time100To200,
                Time200To250 = CreateRequest.Time200To250,
                TimeQuarterMile = CreateRequest.TimeQuarterMile
            };

            _context.Cars.Add(car);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                if (imagePath is not null)
                {
                    await _carImageService.DeleteAsync(imagePath);
                }

                throw;
            }

            return RedirectToPage("./Index");
        }

        private bool IsMultipartFormData()
        {
            return Request.HasFormContentType &&
                   Request.ContentType is not null &&
                   Request.ContentType.StartsWith(
                       "multipart/form-data",
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}
using Carbase.Data;
using Carbase.Models.Car;
using Carbase.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Carbase.Pages.Cars;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IValidator<CarEditRequest> _validator;
    private readonly CarImageService _carImageService;

    public EditModel(
        AppDbContext context,
        IValidator<CarEditRequest> validator,
        CarImageService carImageService)
    {
        _context = context;
        _validator = validator;
        _carImageService = carImageService;
    }

    [BindProperty]
    public CarEditRequest EditRequest { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(long? id)
    {
        if (id is null) return BadRequest();

        var car = await _context.Cars
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (car is null) return NotFound();

        EditRequest = new CarEditRequest
        {
            Id = car.Id,

            ImagePath = car.ImagePath,

            Brand = car.Brand,
            Model = car.Model,
            Year = car.Year,
            Tuner = car.Tuner,

            WeightKg = car.WeightKg,
            HorsePower = car.HorsePower,
            TorqueNm = car.TorqueNm,
            TopSpeed = car.TopSpeed,
            Time0To100 = car.Time0To100,
            Time100To200 = car.Time100To200,
            Time200To250 = car.Time200To250,
            TimeQuarterMile = car.TimeQuarterMile
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(long id)
    {
        if (!IsMultipartFormData())
        {
            return StatusCode(
                StatusCodes.Status415UnsupportedMediaType);
        }

        if (id != EditRequest.Id)
        {
            return BadRequest();
        }

        var car = await _context.Cars.FindAsync(id);

        if (car is null)
        {
            return NotFound();
        }

        EditRequest.Id = car.Id;
        EditRequest.ImagePath = car.ImagePath;
        EditRequest.Brand = car.Brand;
        EditRequest.Model = car.Model;
        EditRequest.Year = car.Year;
        EditRequest.Tuner = car.Tuner;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var validationResult =
            await _validator.ValidateAsync(EditRequest);

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(
                    $"EditRequest.{error.PropertyName}",
                    error.ErrorMessage);
            }

            return Page();
        }

        var oldImagePath = car.ImagePath;
        string? newImagePath = null;

        if (EditRequest.Image is not null)
        {
            try
            {
                newImagePath = await _carImageService.SaveAsync(
                    EditRequest.Image);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    "EditRequest.Image",
                    ex.Message);

                EditRequest.ImagePath = oldImagePath;

                return Page();
            }
        }

        car.WeightKg = EditRequest.WeightKg;
        car.HorsePower = EditRequest.HorsePower;
        car.TorqueNm = EditRequest.TorqueNm;
        car.TopSpeed = EditRequest.TopSpeed;

        car.Time0To100 = EditRequest.Time0To100;
        car.Time100To200 = EditRequest.Time100To200;
        car.Time200To250 = EditRequest.Time200To250;
        car.TimeQuarterMile = EditRequest.TimeQuarterMile;

        if (newImagePath is not null)
        {
            car.ImagePath = newImagePath;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch
        {
            if (newImagePath is not null)
            {
                await _carImageService.DeleteAsync(newImagePath);
            }

            throw;
        }

        if (newImagePath is not null)
        {
            await _carImageService.DeleteAsync(oldImagePath);
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
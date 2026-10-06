using Carbase.Data;
using Carbase.Models.Car;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Carbase.Pages.Cars;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IValidator<CarEditRequest> _validator;

    public EditModel(
        AppDbContext context,
        IValidator<CarEditRequest> validator)
    {
        _context = context;
        _validator = validator;
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

    public async Task<IActionResult> OnPostAsync()
    {
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

        var car = await _context.Cars.FindAsync(EditRequest.Id);

        if (car is null)
        {
            return NotFound();
        }

        car.WeightKg = EditRequest.WeightKg;
        car.HorsePower = EditRequest.HorsePower;
        car.TorqueNm = EditRequest.TorqueNm;
        car.TopSpeed = EditRequest.TopSpeed;

        car.Time0To100 = EditRequest.Time0To100;
        car.Time100To200 = EditRequest.Time100To200;
        car.Time200To250 = EditRequest.Time200To250;
        car.TimeQuarterMile = EditRequest.TimeQuarterMile;

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
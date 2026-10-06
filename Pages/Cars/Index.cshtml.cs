using Carbase.Data;
using Carbase.Models.Car;
using Carbase.Models.Filters;
using Carbase.Models.Sorting;
using Carbase.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Carbase.Pages.Cars
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly CarQueryService _carQueryService;

        public IndexModel(
            AppDbContext context,
            CarQueryService carQueryService)
        {
            _context = context;
            _carQueryService = carQueryService;
        }

        public IList<Car> Cars { get; set; } = new List<Car>();

        [BindProperty(SupportsGet = true)]
        public CarFilterRequest Filter { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public CarSortRequest Sort { get; set; } = new();

        public async Task OnGetAsync()
        {
            var query = _context.Cars
                .AsNoTracking()
                .AsQueryable();

            query = _carQueryService.ApplyFilters(query, Filter);

            query = _carQueryService.ApplySorting(query, Sort);

            Cars = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(long? id)
        {
            if (!ModelState.IsValid || id is null)
            {
                return BadRequest();
            }

            var car = await _context.Cars.FindAsync(id.Value);

            if (car is null)
            {
                return NotFound();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
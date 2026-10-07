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
        private readonly CarImageService _carImageService;

        public IndexModel(
            AppDbContext context,
            CarQueryService carQueryService,
            CarImageService carImageService)
        {
            _context = context;
            _carQueryService = carQueryService;
            _carImageService = carImageService;
        }

        public IList<Car> Cars { get; set; } = new List<Car>();

        [BindProperty(SupportsGet = true)]
        public CarFilterRequest Filter { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public CarSortRequest Sort { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 20;

        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (PageNumber < 1)
            {
                return BadRequest();
            }

            if (PageSize < 1)
            {
                return BadRequest();
            }

            var query = _context.Cars
                .AsNoTracking()
                .AsQueryable();

            query = _carQueryService.ApplyFilters(query, Filter);

            query = _carQueryService.ApplySorting(query, Sort);

            TotalItems = await query.CountAsync();

            TotalPages = (int)Math.Ceiling(
                TotalItems / (double)PageSize);

            Cars = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
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

            var imagePath = car.ImagePath;

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            _carImageService.DeleteFile(imagePath);

            return RedirectToPage();
        }
    }
}
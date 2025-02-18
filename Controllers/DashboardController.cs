using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parcel_Tracking.Data;
using Parcel_Tracking.Models;
using System.Drawing.Printing;
using System.Linq;  // Add this to your Controller file for LINQ functionality


namespace Parcel_Tracking.Controllers
{
    // DashboardController.cs
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            var parcels = await _context.Parcels.ToListAsync();  // Get all parcels
            var totalParcels = _context.Parcels.Count();




            // Start by fetching all parcels
            var parcelsQuery = _context.Parcels.AsQueryable();

            // Apply search filters if search parameters are provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                parcelsQuery = parcelsQuery.Where(p =>
                    p.TrackingNumber.Contains(searchTerm) ||  // Include Tracking Number
                    p.Status.Contains(searchTerm) ||
                    p.RecipientName.Contains(searchTerm) ||
                    p.Address.Contains(searchTerm));
            }











            // Group the parcels by their status and count them
            var parcelStatusCounts = _context.Parcels
                .GroupBy(p => p.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToList();



            // Get the All most recent parcels
            var recentParcels = _context.Parcels
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new
                {
                    p.Status,
                    p.RecipientName,
                    p.Address,
                    p.TrackingNumber,
                    p.ShippingMethod,
                    p.RecipientEmail,
                    p.CreatedAt,
                    p.ShippedAt,
                    p.DeliveredAt,
                    p.CreatedBy,
                    p.Weight
                })
              .ToList();

            // Implement paging
            var filteredParcels = await parcelsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Get total count of parcels (for paging)
            var totalParcelCount = await parcelsQuery.CountAsync(); // Renamed to avoid duplication





            var viewModel = new DashboardsViewModel
            {
                TotalParcels = totalParcelCount,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling((double)totalParcelCount / pageSize),  // Calculate total pages for paging

                StatusCounts = new Dictionary<string, int>
                  {
           { "Pending", _context.Parcels.Count(p => p.Status == "Pending") },
        { "Shipped", _context.Parcels.Count(p => p.Status == "Shipped") },
        { "In Transit", _context.Parcels.Count(p => p.Status == "In Transit") },
        { "Delivered", _context.Parcels.Count(p => p.Status == "Delivered") },
        { "Returned", _context.Parcels.Count(p => p.Status == "Returned") }
        },
                RecentParcels = recentParcels,
                FilteredParcels = parcels.ToList() // Convert IQueryable to List

            };
            ViewData["SearchTerm"] = searchTerm; // Pass the search term to the view
            return View(viewModel);
        }
        // GET: Dashboard/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var parcel = await _context.Parcels.FirstOrDefaultAsync(p => p.Id == id);
            if (parcel == null)
            {
                return NotFound();
            }

            return View(parcel);
        }

    }

}
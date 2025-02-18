using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parcel_Tracking.Data;
using Parcel_Tracking.Models;
using System.Diagnostics;

namespace Parcel_Tracking.Controllers
{
    [Authorize]

   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;



        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;

        }
        public IActionResult Index(string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Fetch the parcel based on the tracking number
                var parcel = _context.Parcels
                    .FirstOrDefault(p => p.TrackingNumber == searchTerm);

                if (parcel != null)
                {
                    return View(parcel);
                }
                else
                {
                    ViewData["Message"] = "Parcel not found.";
                }
            }

            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var parcel = await _context.Parcels.FirstOrDefaultAsync(p => p.Id == id);
            if (parcel == null)
            {
                return NotFound();
            }

            return View(parcel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

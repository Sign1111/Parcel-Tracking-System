using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Parcel_Tracking.Models;
using PayPal.Api;

namespace Parcel_Tracking.Controllers
{
    [Authorize]

    public class ParcelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;



        public ParcelsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;


        }

        // GET: Parcels
        public async Task<IActionResult> Index()
        {
              return _context.Parcels != null ? 
                          View(await _context.Parcels.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Parcels'  is null.");
        }

        // GET: Parcels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Parcels == null)
            {
                return NotFound();
            }

            var parcel = await _context.Parcels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parcel == null)
            {
                return NotFound();
            }

            return View(parcel);
        }

        [Authorize(Policy = "AdminOnly")]

        public async Task<IActionResult> UpdateStatus(int id)
        {
            var request = await _context.Parcels.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        [Authorize(Policy = "AdminOnly")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Parcel request)
        {
            var existing = await _context.Parcels.FindAsync(request.Id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Status = request.Status;
            _context.Update(existing);
            await _context.SaveChangesAsync();


            var user = await _userManager.FindByEmailAsync(existing.CreatedBy);
            if (user != null)
            {
                var message = $"Your service request #{existing.CreatedBy} status was updated to '{existing.Status}'.";
                await _hubContext.Clients.User(user.Id).SendAsync("ReceiveAlert", message);
            }

            TempData["Success"] = "Status updated successfully!";
            return RedirectToAction(nameof(Index));
        }


        // GET: Parcels/Create
        // GET: Parcels/Create
        // GET: Parcels/Create
        // GET: Parcels/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // you can also prefill defaults into the model for GET if you want
            var currentUser = await _userManager.GetUserAsync(User);

            // supply a model with defaults
            var parcel = new Parcel
            {
                CreatedBy = currentUser?.Email,
                CreatedAt = DateTime.Now,
                Status = "Pending"
            };

            return View(parcel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Parcel parcel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                parcel.CreatedBy = currentUser.Email;
                parcel.Status = "Pending"; // force status

                _context.Add(parcel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(parcel);
        }






        // GET: Parcels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Parcels == null)
            {
                return NotFound();
            }

            var parcel = await _context.Parcels.FindAsync(id);
            if (parcel == null)
            {
                return NotFound();
            }
            return View(parcel);
        }

        // POST: Parcels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CreatedBy,CreatedAt,TrackingNumber,RecipientName,Address,Status,Weight,ShippingMethod,ShippedAt,DeliveredAt,RecipientEmail")] Parcel parcel)
        {
            if (id != parcel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parcel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParcelExists(parcel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(parcel);
        }

        // GET: Parcels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Parcels == null)
            {
                return NotFound();
            }

            var parcel = await _context.Parcels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parcel == null)
            {
                return NotFound();
            }

            return View(parcel);
        }

        // POST: Parcels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Parcels == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Parcels'  is null.");
            }
            var parcel = await _context.Parcels.FindAsync(id);
            if (parcel != null)
            {
                _context.Parcels.Remove(parcel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParcelExists(int id)
        {
          return (_context.Parcels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

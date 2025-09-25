using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Parcel_Tracking.Models;
using Polly;

namespace YourNamespace.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _hubContext = hubContext;
        }

        // List all roles
        public async Task<IActionResult> Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        // List all users with their roles
        // List all users with their roles
        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserRolesViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Roles = roles
                });
            }

            return View(userRoles);
        }

        // Remove a role from a user

        [HttpGet]
        [Route("Admin/RemoveRoles/{id}")]

        public async Task<IActionResult> RemoveRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Fetch the user's current roles
            var roles = await _userManager.GetRolesAsync(user);

            var model = new RemoveRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                AssignedRoles = roles.Select(role => new SelectListItem
                {
                    Text = role,
                    Value = role
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/RemoveRoles/{id}")]
        public async Task<IActionResult> RemoveRoles(RemoveRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            if (model.SelectedRoles != null && model.SelectedRoles.Any())
            {
                // Remove selected roles
                var result = await _userManager.RemoveFromRolesAsync(user, model.SelectedRoles);
                if (result.Succeeded)
                {
                    // ✅ Force logout if "Admin" was removed
                    if (model.SelectedRoles.Contains("Admin"))
                    {
                        var session = await _context.UserSessionControls
                            .FirstOrDefaultAsync(s => s.UserId == user.Id);

                        if (session != null)
                        {
                            session.ForceLogout = true;
                            _context.UserSessionControls.Update(session);

                        }
                        else
                        {
                            _context.UserSessionControls.Add(new UserSessionControl
                            {
                                UserId = user.Id,
                                ForceLogout = true
                            });
                        }

                        await _context.SaveChangesAsync();
                        await _hubContext.Clients.User(user.Id).SendAsync("ForceLogout");

                    }

                    TempData["SuccessMessage"] = "Selected roles have been removed successfully.";
                    return RedirectToAction("Dashboard", "Admin");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            else
            {
                ModelState.AddModelError("", "No roles selected for removal.");
            }

            // Reload roles in case of error
            var roles = await _userManager.GetRolesAsync(user);
            model.AssignedRoles = roles.Select(role => new SelectListItem
            {
                Text = role,
                Value = role
            }).ToList();

            return View(model);
        }







        // Create a role
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Role name cannot be empty");
            return View();
        }

        // Assign a role to a user
        public async Task<IActionResult> AssignRole()
        {
            var users = _userManager.Users.ToList();
            var roles = _roleManager.Roles.ToList();

            ViewBag.Users = users;
            ViewBag.Roles = roles;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && !string.IsNullOrWhiteSpace(roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
                return RedirectToAction("ManageUsers");
            }
            ModelState.AddModelError("", "Invalid user or role");
            return View();
        }


    }
}

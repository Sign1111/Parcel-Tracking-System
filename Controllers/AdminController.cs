using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Parcel_Tracking.Models;
using Microsoft.EntityFrameworkCore; // For .Include and .ToListAsync
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Parcel_Tracking;
using Parcel_Tracking.Data;


[Authorize(Policy = "AdminOnly")]

public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;



    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)

    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }




    //EditPanel
    [HttpGet]
    public async Task<IActionResult> EditRoles(string id)
    {
        // Find the user by ID
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("User not found");
        }

        // Get the current roles of the user
        var userRoles = await _userManager.GetRolesAsync(user);

        // Get all roles in the system
        var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

        // Build the view model
        var model = new EditRolesViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            AssignedRoles = allRoles.Select(role => new SelectListItem
            {
                Value = role,
                Text = role,
                Selected = userRoles.Contains(role)
            }).ToList()
        };

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> EditRoles(EditRolesViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId); // Removed 'public'
        if (user == null)
        {
            return NotFound("User not found");
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Failed to remove existing roles");
            return View(model);
        }

        result = await _userManager.AddToRolesAsync(user, model.SelectedRoles);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Failed to assign new roles");
            return View(model);
        }

        TempData["SuccessMessage"] = "Roles updated successfully!";
        return RedirectToAction("Dashboard");
    }

    // Post method to delete user
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRoles(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "User deleted successfully!";
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return RedirectToAction("Dashboard");
    }




    [HttpGet]

    public async Task<IActionResult> Dashboard()
    {


        // Get total number of users
        var totalUsers = await _userManager.Users.CountAsync();

        // Get total number of roles
        var totalRoles = await _roleManager.Roles.CountAsync();

        //Get total number of AssignRole
        var totalAssignRole = await _roleManager.Roles.CountAsync();



        // Pass the data to the view
        ViewBag.TotalUsers = totalUsers;
        ViewBag.TotalRoles = totalRoles;

        var users = _userManager.Users.ToList();
        var userList = new List<DashboardViewModel>();

        {
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new DashboardViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }
            ViewBag.TotalRoles = _roleManager.Roles.Count(); // Total roles for the card
            return View(userList);
        }


    }
}









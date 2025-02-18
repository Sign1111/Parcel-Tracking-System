using Microsoft.AspNetCore.Mvc.Rendering;

public class RemoveRolesViewModel
{
    public string UserId { get; set; } // User's ID
    public string UserName { get; set; } // User's Name
    public List<SelectListItem> AssignedRoles { get; set; } // All assigned roles
    public List<string> SelectedRoles { get; set; } // Roles to be removed
}

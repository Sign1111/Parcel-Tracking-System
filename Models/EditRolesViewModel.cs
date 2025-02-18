using Microsoft.AspNetCore.Mvc.Rendering;

public class EditRolesViewModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public List<SelectListItem> AssignedRoles { get; set; }
    public List<string> SelectedRoles { get; set; }
}

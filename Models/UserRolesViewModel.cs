using Microsoft.AspNetCore.Mvc.Rendering;

public class UserRolesViewModel
{
    public string Id { get; set; }

    // Basic User Information
    public string UserName { get; set; }
    public string Email { get; set; }
    public IList<string> Roles { get; internal set; }

    // Role Management

}

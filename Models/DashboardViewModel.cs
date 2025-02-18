namespace Parcel_Tracking
{


    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalRoles { get; set; }
        public int TotalAssignRole { get; set; }
        public List<UserSummary> RecentUsers { get; set; }
        public string? UserName { get; internal set; }
        public string Id { get; internal set; }
        public string? Email { get; internal set; }
        public List<string> Roles { get; internal set; }
    }

    public class UserSummary
    {
        public string Id { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }

  
}
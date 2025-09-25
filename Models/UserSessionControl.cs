namespace Parcel_Tracking.Models
{
    public class UserSessionControl
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public bool ForceLogout { get; set; } = false;
    }
}

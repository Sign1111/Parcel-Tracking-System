namespace Parcel_Tracking.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public int Id { get; internal set; }
        public object Name { get; internal set; }
        public object Email { get; internal set; }
    }
}
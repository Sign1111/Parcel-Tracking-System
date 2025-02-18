namespace Parcel_Tracking.Models
{
    public class Project
    {
        public int Id { get; set; }

        public string Parcel_Tracking_Number { get; set; }
        public string Parcel_Type { get; set; }
        public string Parcel_Weight { get; set; }
        public string Parcel_Location { get; set; }
        public string Parcel_Final_Destination { get; set; }
        public string Driver_Email { get; set; }

        public DateTime? LockoutEnd { get; set; }



    }
}

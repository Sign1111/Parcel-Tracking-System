using Parcel_Tracking.Models;


    public class DashboardsViewModel
    {
        public int TotalParcels { get; set; }

    public Dictionary<string, int> StatusCounts { get; set; }
    public List<Parcel> Parcels { get; set; }  // List of Parcels
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public List<Parcel> FilteredParcels { get; set; } // Add this property

    public IEnumerable<dynamic> RecentParcels { get; set; } // Recent parcels


}


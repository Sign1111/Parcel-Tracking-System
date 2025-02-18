namespace Parcel_Tracking.Models
{

 


    
    public class Parcel
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; } // Track the admin or user who created the parcel
        public DateTime CreatedAt { get; set; }
        public string TrackingNumber { get; set; }
        public string RecipientName { get; set; } // Name of the recipient
        public string Address { get; set; } // Delivery address
        public string Status { get; set; }
        public string Weight { get; set; } // Weight of the parcel

        public string ShippingMethod { get; set; } // Shipping method (e.g., Standard, Express)
        public DateTime? ShippedAt { get; set; } // Date when the parcel was shipped
        public DateTime? DeliveredAt { get; set; } // Date when the parcel was delivered
        public string RecipientEmail { get; set; }
    }

}





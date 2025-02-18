using System;

namespace Parcel_Tracking.Controllers
{
 

    public static class TrackingNumberGenerator
    {
        public static string GenerateTrackingNumber()
        {
            // Example: Generates a tracking number in the format "TRK-20250204-123456"
            string prefix = "TRK";
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string randomPart = new Random().Next(100000, 999999).ToString();

            return $"{prefix}-{datePart}-{randomPart}";
        }
    }

}

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Parcel_Tracking.Models
{
    public class ProfileViewModel
    {
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }

        public string ExistingProfilePicturePath { get; set; } // If you want to display existing profile picture
    }
}

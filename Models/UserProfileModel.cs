using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace Parcel_Tracking.Models
{
    public class UserProfileModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;

        public UserProfileModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [BindProperty]
        public string? ProfileImageUrl { get; set; }  // Add this property to store image path

        [BindProperty]
        public IFormFile? ProfileImage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ProfileImage != null)
            {
                // Define the upload path
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Save the image
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }

                // Set the ProfileImageUrl to the relative path
                ProfileImageUrl = $"/uploads/{fileName}";
            }

            return Page();
        }
    }
}
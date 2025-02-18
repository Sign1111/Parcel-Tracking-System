using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

public class IndexModel : PageModel
{
    private readonly IWebHostEnvironment _hostEnvironment;

    public IndexModel(IWebHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string PhoneNumber { get; set; }

    [BindProperty]
    public IFormFile ProfileImage { get; set; }

    public string ImagePath { get; set; }
    public string StatusMessage { get; set; }

    public void OnGet()
    {
        // Retrieve data from session if available
        Username = HttpContext.Session.GetString("Username");
        PhoneNumber = HttpContext.Session.GetString("PhoneNumber");
        ImagePath = HttpContext.Session.GetString("ImagePath");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ProfileImage != null)
        {
            // Store the image and get its path
            var filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", ProfileImage.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ProfileImage.CopyToAsync(stream);
            }

            ImagePath = "/images/" + ProfileImage.FileName;

            // Store in session
            HttpContext.Session.SetString("ImagePath", ImagePath);
        }

        // Store username and phone number in session
        HttpContext.Session.SetString("Username", Username);
        HttpContext.Session.SetString("PhoneNumber", PhoneNumber);

        StatusMessage = "Profile updated successfully!";
        return RedirectToPage();  // Reload the page to reflect changes
    }
}

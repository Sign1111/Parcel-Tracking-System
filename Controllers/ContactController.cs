using Microsoft.AspNetCore.Mvc;

namespace Parcel_Tracking.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

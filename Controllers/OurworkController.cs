using Microsoft.AspNetCore.Mvc;

namespace Parcel_Tracking.Controllers
{
    public class OurworkController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

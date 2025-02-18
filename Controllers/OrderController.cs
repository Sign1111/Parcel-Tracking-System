using Microsoft.AspNetCore.Mvc;

namespace Parcel_Tracking.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}

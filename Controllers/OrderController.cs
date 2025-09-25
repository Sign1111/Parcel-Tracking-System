using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Parcel_Tracking.Controllers
{
    [Authorize]


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

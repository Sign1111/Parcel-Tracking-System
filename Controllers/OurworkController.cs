using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Parcel_Tracking.Controllers
{
    [Authorize]

    public class OurworkController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

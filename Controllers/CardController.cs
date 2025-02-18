using Microsoft.AspNetCore.Mvc;

namespace Parcel_Tracking.Controllers
{
    public class CardController : Controller
    {

        [HttpPost]
        public IActionResult ProcessPayment(decimal amount)
        {
            if (amount <= 0)
            {
                return Content("Invalid payment amount!");
            }

            // Process the payment logic here
            return Content($"Card payment of ${amount} processed successfully!");
        }



        public IActionResult Index()
        {
            return View();
        }
    }
}

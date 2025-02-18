using Microsoft.AspNetCore.Mvc;

public class PayPalController : Controller
{
    private readonly PayPalService _payPalService;

    public PayPalController(PayPalService payPalService)
    {
        _payPalService = payPalService;
    }
    public IActionResult ExecutePayment(string paymentId, string token, string PayerID)
    {
        if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(PayerID))
        {
            return BadRequest("Missing required payment details. Please try again.");
        }

        var payment = _payPalService.ExecutePayment(paymentId, PayerID);

        if (payment == null)
        {
            // Handle failed payment execution
            return View("PaymentFailed");
        }

        return View("PaymentSuccess", payment);
    }

    // Action to show the payment amount input form
    [HttpGet]
    public IActionResult EnterAmount()
    {
        return View();
    }

    // Action to create a payment after user submits the amount
    [HttpPost]
    public IActionResult CreatePayment(decimal amount)
    {
        try
        {
            if (amount <= 0)
            {
                ModelState.AddModelError("", "Amount must be greater than 0.");
                return View("EnterAmount");
            }

            var returnUrl = Url.Action("ExecutePayment", "PayPal", null, Request.Scheme);
            var cancelUrl = Url.Action("Cancel", "PayPal", null, Request.Scheme);

            // Create PayPal payment
            var payment = _payPalService.CreatePayment(returnUrl, cancelUrl, amount);

            var approvalUrl = payment.links.FirstOrDefault(link => link.rel == "approval_url")?.href;

            if (approvalUrl == null)
                throw new Exception("Approval URL not found.");

            return Redirect(approvalUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating PayPal payment: {ex.Message}");
            return BadRequest("An error occurred while creating the payment. Please try again later.");
        }
    }

    public IActionResult CompletePayment(string paymentId, string token, string PayerID)
    {
        var payment = _payPalService.ExecutePayment(paymentId, PayerID);
        return View("PaymentSuccess", payment);
    }



    public IActionResult Cancel()
    {
        return View("PaymentCancelled");
    }
}


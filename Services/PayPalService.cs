using PayPal.Api;
using Microsoft.Extensions.Options;
using Parcel_Tracking.Models;


public class PayPalService
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _mode;
    private readonly PayPalOptions _options;


    public PayPalService(IConfiguration configuration, IOptions<PayPalOptions> options)
    {
        _clientId = configuration["Adbr31ad0AYUrVSj-HJAwaYy6i5t96IxDEIGke-hTJwLGi_-7drKkZY65AxhEMHEAbKkvi2D8_zMOS6l"];
        _clientSecret = configuration["EGS5E9U60WaSjem3xcd2MAVXfBeMdqVe0gbRu80Z7z1QhpAEAut2k0WHmt8JPXPA9Ubtjit-dJ085aLd"];
        _mode = configuration["sandbox"];
        _options = options.Value;

    }

    private APIContext GetApiContext()
    {
        var config = new Dictionary<string, string>
   {
    { "mode", "sandbox" }, // Or "live"
    { "log.LogLevel", "FINE" },
    { "log.FileName", "PayPal.log" },
    { "log.FilePath", "C:\\logs\\" }
     };

        Console.WriteLine($"ClientId: {_options.ClientId}, Mode: {_options.Mode}");

        var accessToken = new OAuthTokenCredential(_options.ClientId, _options.ClientSecret, config).GetAccessToken();
        return new APIContext(accessToken) { Config = config };
    }


    public Payment CreatePayment(string returnUrl, string cancelUrl, decimal amount)
    {
        var apiContext = GetApiContext();

        var payment = new Payment
        {
            intent = "sale",
            payer = new Payer { payment_method = "paypal" },
            transactions = new List<Transaction>
        {
            new Transaction
            {
                description = "Payment for User-Specified Amount",
                invoice_number = Guid.NewGuid().ToString(),
                amount = new Amount
                {
                    currency = "USD",
                    total = amount.ToString("F2") // Format amount to 2 decimal places
                }
            }
        },
            redirect_urls = new RedirectUrls
            {
                return_url = returnUrl,
                cancel_url = cancelUrl
            }
        };

        return payment.Create(apiContext);
    }


    public Payment ExecutePayment(string paymentId, string payerId)
    {
        var apiContext = GetApiContext();

        var paymentExecution = new PaymentExecution { payer_id = payerId };
        var payment = new Payment { id = paymentId };

        return payment.Execute(apiContext, paymentExecution);
    }
}
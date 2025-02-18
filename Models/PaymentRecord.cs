public class PaymentRecord
{
    public int Id { get; set; }
    public string PaymentId { get; set; }
    public string PayerId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

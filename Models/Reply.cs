namespace Parcel_Tracking.Models
{
    public class Reply
    {
        public int Id { get; set; }
        public int MessageId { get; set; } // Foreign key to Message
        public string Sender { get; set; }

        public string Receiver { get; set; }  // 👈 ADD THIS

        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public Message Message { get; set; } // Navigation property
    }
}

namespace Parcel_Tracking.Models
{
    public class ReplyViewModel
    {
        public int Id { get; set; }


        public int MessageId { get; set; }

        public string Sender { get; set; }  // The user replying

        public string Receiver { get; set; }
        public string Content { get; set; }
    }
}

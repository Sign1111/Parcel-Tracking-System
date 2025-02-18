

using System.ComponentModel.DataAnnotations.Schema;

namespace Parcel_Tracking.Models
{


    public class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public ICollection<Reply> Replies { get; set; }
        public int? ReplyToMessageId { get; set; } // Nullable for original messages


        [ForeignKey("ReplyToMessageId")]
        public virtual Message? ReplyTo { get; set; } // Navigation property
        public bool IsRead { get; internal set; }
    }

   
}
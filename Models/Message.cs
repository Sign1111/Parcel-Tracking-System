using System;
using System.Collections.Generic;
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

        // For the “child” messages that reply to this message:
        public ICollection<Message> Replies { get; set; } = new List<Message>();

        // For the “parent” message this one replies to:
        public int? ReplyToMessageId { get; set; }

        [ForeignKey(nameof(ReplyToMessageId))]
        public virtual Message? ReplyTo { get; set; }

        public bool IsRead { get; set; }
    }
}

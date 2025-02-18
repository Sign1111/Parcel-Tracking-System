namespace Parcel_Tracking.Models
{
    public class ChatViewModel
    {
        public List<string> UserList { get; set; }
        public List<Message> Messages { get; set; }
        public string SelectedUserId { get; set; }
    }
}

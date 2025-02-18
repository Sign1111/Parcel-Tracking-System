using Microsoft.AspNetCore.SignalR;

namespace Parcel_Tracking.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string sender, string receiver, string messageContent)
        {
            await Clients.User(receiver).SendAsync("ReceiveMessage", sender, messageContent);
        }
    }
}

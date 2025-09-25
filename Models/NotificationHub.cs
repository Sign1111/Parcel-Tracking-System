using Microsoft.AspNetCore.SignalR;
using Polly;
using System.Text.RegularExpressions;

namespace Parcel_Tracking.Models
{
    public class NotificationHub : Hub
    {
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task SendAlert(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveAlert", message);
        }
    }

}

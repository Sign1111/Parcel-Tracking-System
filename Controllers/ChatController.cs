using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parcel_Tracking.Data;
using Parcel_Tracking.Models; // Ensure you include this if Message class is in this namespace


namespace Parcel_Tracking.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public ChatController(ApplicationDbContext context, UserManager<IdentityUser> userManager)

        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Admin Inbox
        [Authorize(Roles = "Admin")]

        public IActionResult AdminInbox()
        {
            // Get the list of users that the Admin can chat with.
            // This could be fetching user names or user IDs from your database.
            var users = _context.Users.Select(u => u.UserName).ToList();

            return View(users); // Pass the list of users to the AdminInbox view
        }
        public IActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public IActionResult SendMessage(string userId)
        {

            ViewBag.UserId = userId; // Pass the selected user's ID to the view
            return View();
        }

        [HttpPost]
        public IActionResult SendMessage(string userId, string messagebox)
        {
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("UserId", "UserId cannot be empty.");
                return View();
            }

            if (string.IsNullOrEmpty(messagebox))
            {
                ModelState.AddModelError("Message", "Message cannot be empty.");
                return View();
            }

            // Save the message 
            var message = new Message
            {
                Sender = User.Identity.Name, // Admin username
                Receiver = userId,

                Content = $" :{messagebox}",
                SentAt = DateTime.Now,
                IsRead = false               // Set IsRead to false

            };

            _context.Messages.Add(message); // Add the message to the database
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Message sent successfully.";

            // Redirect back to the Admin's inbox or another relevant page
            return RedirectToAction("AdminInbox");
        }





        // GET: Reply Message
        [HttpGet]
        public async Task<IActionResult> Reply(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            var replyModel = new ReplyViewModel
            {
                MessageId = message.Id,
                Receiver = message.Sender,  // Reply goes to the original sender
                Sender = User.Identity.Name,  // Current logged-in user
                Content = "Reply"
            };

            return View(replyModel);
        }




        // POST: Send Reply

        [HttpPost]
        public async Task<IActionResult> Reply(int MessageId, string Content)
        {
            if (string.IsNullOrWhiteSpace(Content))
            {
                return BadRequest("Reply content cannot be empty.");
            }

            var originalMessage = _context.Messages.Find(MessageId);
            if (originalMessage == null)
            {
                return NotFound();
            }



            var replyMessage = new Message
            {
                Sender = User.Identity.Name,  // Assuming Identity is used
                Receiver = originalMessage.Sender, // Reply goes back to the sender
                Content = "Replying to: \"" + originalMessage.Content + "\"\n\n" + Content, // Shows reply reference
                SentAt = DateTime.Now,
                ReplyToMessageId = MessageId // Link to the original message
            };

            _context.Messages.Add(replyMessage);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "✅ Reply sent successfully.";


            return RedirectToAction("UserInbox");
        }




        public IActionResult UserInbox()
        {
            string currentUserId = User.Identity.Name;

            // Fetch all messages sent to the current user, including replies
            var messages = _context.Messages
                .Include(m => m.ReplyTo) // Ensure replies are loaded to avoid null reference
                .Where(m => m.Receiver == currentUserId)
                .OrderByDescending(m => m.SentAt)
                .ToList();

            return View(messages);
        }



        [HttpGet]
        public IActionResult SendTrackingNumber(string userId)
        {

            ViewBag.UserId = userId; // Pass the selected user's ID to the view
            return View();
        }

        [HttpPost]
        public IActionResult SendTrackingNumber(string userId, string trackingNumber)
        {
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("UserId", "UserId cannot be empty.");
                return View();
            }

            if (string.IsNullOrEmpty(trackingNumber))
            {
                ModelState.AddModelError("TrackingNumber", "Tracking Number cannot be empty.");
                return View();
            }

            // Save the message as a tracking number sent from the Admin to the User
            var message = new Message
            {
                Sender = User.Identity.Name, // Admin username
                Receiver = userId,
                Content = $"Tracking Number: {trackingNumber}",
                SentAt = DateTime.Now,
                IsRead = false               // Set IsRead to false

            };

            _context.Messages.Add(message); // Add the message to the database
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Tracking number sent successfully.";

            // Redirect back to the Admin's inbox or another relevant page
            return RedirectToAction("AdminInbox");
        }

        public IActionResult UserDashboard()
        {
            string currentUserId = User.Identity.Name;

            // Fetch unread messages for the current user
            var unreadMessages = _context.Messages
                                        .Where(m => m.Receiver == currentUserId && !m.IsRead)
                                        .OrderByDescending(m => m.SentAt)  // Optionally order by latest
                                        .ToList();

            // Count the unread messages
            var unreadMessagesCount = unreadMessages.Count();

            ViewBag.UnreadMessagesCount = unreadMessagesCount;

            // Check if the request is an AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // If it's an AJAX request, return a partial view
                return PartialView("_UserDashboardMessages", unreadMessages); // Only return the updated message list
            }

            // If it's a normal request, return the full view
            return View(unreadMessages);
        }






        [HttpPost]
        public IActionResult MarkAsRead(int messageId)
        {
            var message = _context.Messages.Find(messageId);

            if (message != null)
            {
                message.IsRead = true;
                _context.SaveChanges();
            }

            return RedirectToAction("UserDashboard");
        }



        // This action will return the latest messages (you can customize it)
        [HttpGet]
        public IActionResult GetNewMessages()
        {
            string currentUserId = User.Identity.Name;

            // Get the latest unread messages
            var messages = _context.Messages
                .Where(m => m.Receiver == currentUserId && !m.IsRead)
                .OrderByDescending(m => m.SentAt)
                .Take(50)  // Limit the number of messages (adjust as needed)
                .ToList();

            return PartialView("_UserDashboardMessages", messages);  // Return a partial view with the updated messages
        }


        [HttpPost]
        public async Task<IActionResult> DeleteMessage(int MessageId)
        {
            var message = await _context.Messages.FindAsync(MessageId);

            if (message == null)
            {
                return NotFound();
            }

            // Check if the message has replies
            bool hasReplies = await _context.Messages.AnyAsync(m => m.ReplyToMessageId == MessageId);

            if (hasReplies)
            {
                TempData["ErrorMessage"] = "Cannot delete this message because it has replies.";
                return RedirectToAction("UserInbox"); // Redirect back with an error message
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Message deleted successfully.";
            return RedirectToAction("UserInbox");
        }





    }
}

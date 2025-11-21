using EmployeeManage.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EmployeeManage.Hubs
{
    // The [Authorize] attribute should be used here to ensure only logged-in users can connect
    [Authorize] 
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        // Called when a message is sent from the client
        public async Task SendMessage(string receiverUserName, string messageContent)
        {
            // Get current logged-in user's username (sender) from the connection context
            var senderUserName = Context.User?.Identity?.Name;

            // CRITICAL: Basic check to ensure a sender is identified
            if (string.IsNullOrEmpty(senderUserName))
            {
                // Optionally handle or log error if sender is unknown/unauthenticated
                return;
            }

            var sentAt = DateTime.UtcNow; // Use UTC for consistency

            // 1. SAVE THE MESSAGE TO THE DATABASE
            var messageEntity = new Message
            {
                Sender = senderUserName,
                Receiver = receiverUserName,
                Content = messageContent,
                SentAt = sentAt,
                IsRead = false // It's unread until the receiver views it
            };

            try
            {
                _context.Messages.Add(messageEntity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving message: " + ex.Message);
            }


            // 2. BROADCAST THE MESSAGE

            // Send to the target user (receiver)
            // Note: The CustomUserIdProvider uses the UserName as the User ID
            await Clients.User(receiverUserName)
                .SendAsync("ReceiveMessage", senderUserName, messageContent, sentAt.ToLocalTime().ToString("HH:mm:ss"));

            // Also send it back to the sender’s own chat window (Clients.Caller)
            await Clients.Caller
                .SendAsync("ReceiveMessage", senderUserName, messageContent, sentAt.ToLocalTime().ToString("HH:mm:ss"));
            Console.WriteLine($"Message from {senderUserName} to {receiverUserName}: {messageContent}");

        }
    }
}
using EmployeeManage.Data;
using EmployeeManage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManage.Pages
{
    [Authorize] // Ensures only logged-in users can access chat
    public class ChatModel : PageModel
    {
        private readonly AppDbContext _context;

        public ChatModel(AppDbContext context)
        {
            _context = context;
        }

        // List to hold chat messages between the current user and another user
        public List<Message> Messages { get; set; } = new();

        // The username of the person you’re chatting with
        public string? ChatWith { get; set; }

        public async System.Threading.Tasks.Task OnGetAsync(string? withUser)
        {
            var currentUser = User.Identity?.Name;

            // Skip loading if user not logged in or no chat partner specified
            if (string.IsNullOrEmpty(currentUser) || string.IsNullOrEmpty(withUser))
                
              return;

            ChatWith = withUser;

            // Load chat history between current user and target user
            Messages = await _context.Messages
                .Where(m =>
                    (m.Sender == currentUser && m.Receiver == withUser) ||
                    (m.Sender == withUser && m.Receiver == currentUser))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            // Optional: Mark unread messages as read
            var unread = await _context.Messages
                .Where(m => m.Receiver == currentUser && m.Sender == withUser && !m.IsRead)
                .ToListAsync();

            foreach (var msg in unread)
                msg.IsRead = true;

            if (unread.Any())
                await _context.SaveChangesAsync();

        }
        [HttpGet]
        public async Task<JsonResult> OnGetAllMessagesAsync()
        {
            var currentUser = User.Identity?.Name;

            var messages = await _context.Messages
                .Where(m => m.Sender == currentUser || m.Receiver == currentUser)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return new JsonResult(messages);
        }
        [HttpGet]
        public async Task<JsonResult> OnGetReceivedMessagesAsync()
        {
            var currentUser = User.Identity?.Name;

            var messages = await _context.Messages
                .Where(m => m.Receiver == currentUser)
                .OrderByDescending(m => m.SentAt)
                .Take(10)
                .Select(m => new {
                    m.Sender,
                    m.Content,
                    m.SentAt,
                    m.IsRead
                })
                .ToListAsync();

            return new JsonResult(messages);
        }


    }
}

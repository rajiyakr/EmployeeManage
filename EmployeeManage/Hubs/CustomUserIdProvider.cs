using Microsoft.AspNetCore.SignalR;

namespace EmployeeManage.Hubs
{
    // This tells SignalR how to identify users uniquely (by username/email)
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            // This uses the logged-in user's identity (like email)
            return connection.User?.Identity?.Name;
        }
    }
}

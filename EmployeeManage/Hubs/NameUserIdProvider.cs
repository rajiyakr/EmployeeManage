using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

public class NameUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        // Use email or username as unique key
        return connection.User?.Identity?.Name;
    }
}

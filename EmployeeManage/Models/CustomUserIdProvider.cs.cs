using Microsoft.AspNetCore.SignalR;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        // This will use the username (email) as the SignalR UserId
        return connection.User?.Identity?.Name;
    }
}

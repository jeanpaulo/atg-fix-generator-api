using Microsoft.AspNetCore.SignalR;
using QuickFix.Fields;

namespace OrderGenerator.API.hub;

public sealed class FixHub : Hub<IFixHub>
{
    public override async Task OnConnectedAsync()
    {
        //await this.Clients.All.ReceiveMessage($"{Context.ConnectionId} has joined.");
    }

    public async Task SendMessage(string message)
    {
        await Clients.All.ReceiveMessage($"{Context.ConnectionId}: {message}");
    }
}


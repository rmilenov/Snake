using Microsoft.AspNetCore.SignalR;

namespace SnakeServer.Hubs
{
    public class SnakeHub:Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task SendGameState(string gameState)
        {
            await Clients.All.SendAsync("ReceiveGamestate", gameState);
        }
    }
}

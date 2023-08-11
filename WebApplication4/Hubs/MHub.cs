using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebApplication4.Hubs
{
    //[Authorize]
    public class MmHub : Hub
        {
           private readonly ILogger<MmHub> _logger;

           public MmHub(ILogger<MmHub> logger)
           {
            _logger = logger;

           }
        private static List<string> ConnectedClients = new List<string>();

        public override async Task OnConnectedAsync()
        {
            ConnectedClients.Add(Context.ConnectionId);
            await Clients.All.SendAsync("clients", ConnectedClients);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ConnectedClients.Remove(Context.ConnectionId);
            await Clients.All.SendAsync("clients", ConnectedClients);
        }

        public List<string> GetConnectedClients()
        {
            return ConnectedClients;
        }
     
        public async Task SendOrderNotification(string message)
        {
                await Clients.All.SendAsync("ReceiveOrderNotification", message);
        }
    }
    
}

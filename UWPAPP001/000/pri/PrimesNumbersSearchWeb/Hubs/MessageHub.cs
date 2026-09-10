using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.SignalR;

namespace PrimesNumbersSearchWeb.Hubs
{
    public class MessageHub : Hub
    {
        protected IHubContext<MessageHub> _context;
        public MessageHub(IHubContext<MessageHub> context)
        {
            this._context = context;
            _context = context;
        }

        private static List<UserConn> _connectedUsers = new List<UserConn>();

        public class UserConn
        {
            public String username { get; set; }
            public String connId { get; set; }
        }

        public override Task OnConnectedAsync()
        {
            var username = Context.GetHttpContext().Request.Query["username"];

            var status = _connectedUsers.FirstOrDefault(x => x.username == username);

            if (status == null)
            {
                _connectedUsers.Add(new UserConn
                {
                    connId = Context.ConnectionId,
                    username = username
                });

            }

            return base.OnConnectedAsync();
        }

        public async Task SendMessage(string user, string message)
        {
            //if (string.IsNullOrEmpty(connId))
            //    await _context.Clients.All.SendAsync("ReceiveMessageHandler", message);
            //else

            _connectedUsers.ForEach(val =>
            {
                if (val.username == user)
                    _context.Clients.Clients(val.connId).SendAsync("ReceiveMessageHandler", message);
            });

            //await _context.Clients.Clients(Context.ConnectionId).SendAsync("ReceiveMessageHandler", message);
        }

        public async Task SendMessageAll(string message)
        {
            await _context.Clients.All.SendAsync("ReceiveMessageHandler", message);
        }
    }
}

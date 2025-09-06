using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MissionControlSimulator.src.Hubs
{
    public class MissionHub : Hub
    {
        public async Task SendMissionUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveMissionUpdate", message);
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MissionControlSimulator.src.Hubs
{
    public class MissionHub : Hub
    {
        // פונקציה שמקבלת הודעה מהלקוח ושולחת לכולם
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}

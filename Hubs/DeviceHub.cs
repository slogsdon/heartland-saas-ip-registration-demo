using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SaasIPRegistration.Demo.Models;

namespace SaasIPRegistration.Demo.Hubs
{
    public class DeviceHub : Hub
    {
        public async Task SendMessage(DeviceDetails details)
        {
            await Clients.All.SendAsync("ReceiveDeviceRegistration", details);
        }
    }
}
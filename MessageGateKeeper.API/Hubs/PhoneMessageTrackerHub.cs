using MessageGateKeeper.API.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace MessageGateKeeper.API.Hubs
{
    public class PhoneMessageTrackerHub : Hub<IPhoneMessageTrackerHub>
    {
    }
}

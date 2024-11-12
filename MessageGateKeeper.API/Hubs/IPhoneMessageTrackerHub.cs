using MessageGateKeeper.API.Models;
using System.Collections.Concurrent;

namespace MessageGateKeeper.API.Hubs
{
    public interface IPhoneMessageTrackerHub
    {
        Task BroadcastPhoneMessageTracker(ConcurrentDictionary<string, PhoneMessageTracker> phoneNumberMessages);
    }
}

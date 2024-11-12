using MessageGateKeeper.API.Hubs;
using MessageGateKeeper.API.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace MessageGateKeeper.API.Services
{
    public class RateLimiterService : IRateLimiterService, IDisposable
    {
        private readonly RateLimitConfig _config;
        private readonly ConcurrentDictionary<string, PhoneMessageTracker> _phoneNumberMessages;
        private readonly Queue<DateTime> _accountMessages;
        private readonly TimeSpan _phoneNumberExpiration;
        private readonly Timer _cleanupTimer;

        private readonly IHubContext<PhoneMessageTrackerHub, IPhoneMessageTrackerHub> _signalRContext;

        public RateLimiterService(RateLimitConfig config, IHubContext<PhoneMessageTrackerHub, IPhoneMessageTrackerHub> signalRContext = null)
        {
            _config = config;
            _phoneNumberMessages = new ConcurrentDictionary<string, PhoneMessageTracker>();
            _accountMessages = new Queue<DateTime>();
            _signalRContext = signalRContext;

            _phoneNumberExpiration = TimeSpan.FromMinutes(1);
            _cleanupTimer = new Timer(callback: CleanupInactivePhoneNumbers, new AutoResetEvent(false), TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        public bool CanSendMessage(string phoneNumber)
        {
            if(string.IsNullOrEmpty(phoneNumber))
                return false;

            var now = DateTime.UtcNow;

            // Check per-phone-number limit
            var tracker = _phoneNumberMessages.GetOrAdd(phoneNumber, _ => new PhoneMessageTracker());
            lock (tracker.MessageTimestamps)
            {
                tracker.LastActivity = now;

                // Remove timestamps older than 1 second
                while (tracker.MessageTimestamps.Count > 0 && (now - tracker.MessageTimestamps.Peek()).TotalSeconds >= 1)
                    tracker.MessageTimestamps.Dequeue();

                if (tracker.MessageTimestamps.Count >= _config.MaxMessagesPerPhoneNumberPerSecond)
                    return false;

                tracker.MessageTimestamps.Enqueue(now);
                if (_signalRContext != null)
                {
                    //broad cast updated messages so that client can see the updated messages
                    _signalRContext.Clients.All.BroadcastPhoneMessageTracker(_phoneNumberMessages);
                }
               
            }

            // Check account-wide limit
            lock (_accountMessages)
            {
                while (_accountMessages.Count > 0 && (now - _accountMessages.Peek()).TotalSeconds >= 1)
                    _accountMessages.Dequeue();

                if (_accountMessages.Count >= _config.MaxMessagesPerAccountPerSecond)
                    return false;

                _accountMessages.Enqueue(now);
            }

            return true;
        }

        // Periodic cleanup to remove inactive phone numbers
        public void CleanupInactivePhoneNumbers(object state)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)state;
            var expirationTime = DateTime.UtcNow - _phoneNumberExpiration;

            foreach (var key in _phoneNumberMessages.Keys)
            {
                if (_phoneNumberMessages.TryGetValue(key, out var tracker) && tracker.LastActivity > expirationTime)
                {
                    _phoneNumberMessages.TryRemove(key, out _);
                }
            }
            if (_signalRContext != null)
            {
                //broad cast updated messages so that client can see the updated messages
                _signalRContext.Clients.All.BroadcastPhoneMessageTracker(_phoneNumberMessages);
            }
        }

        public void Dispose()
        {
            _cleanupTimer.Dispose();
        }
    }

}

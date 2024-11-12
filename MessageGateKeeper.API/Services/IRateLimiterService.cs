namespace MessageGateKeeper.API.Services
{
    public interface IRateLimiterService : IDisposable
    {
        bool CanSendMessage(string phoneNumber);
    }
}

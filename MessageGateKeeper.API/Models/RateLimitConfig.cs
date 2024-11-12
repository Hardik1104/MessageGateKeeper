namespace MessageGateKeeper.API.Models
{
    public class RateLimitConfig
    {
        public int MaxMessagesPerPhoneNumberPerSecond { get; set; }
        public int MaxMessagesPerAccountPerSecond { get; set; }
    }
}

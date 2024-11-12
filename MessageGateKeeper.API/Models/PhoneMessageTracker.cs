namespace MessageGateKeeper.API.Models
{
    public class PhoneMessageTracker
    {
        public Queue<DateTime> MessageTimestamps { get; } = new Queue<DateTime>();
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    }
}

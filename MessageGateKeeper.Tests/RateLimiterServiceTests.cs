using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageGateKeeper.API.Hubs;
using MessageGateKeeper.API.Models;
using MessageGateKeeper.API.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace MessageGateKeeper.Tests
{
    public class RateLimiterServiceTests
    {
        private readonly RateLimiterService _rateLimiterService;
        private readonly RateLimitConfig _config;
        //private readonly IHubContext<PhoneMessageTrackerHub, IPhoneMessageTrackerHub> _signalRContext;

        public RateLimiterServiceTests()
        {
            _config = new RateLimitConfig
            {
                MaxMessagesPerPhoneNumberPerSecond = 2,
                MaxMessagesPerAccountPerSecond = 5
            };

            _rateLimiterService = new RateLimiterService(_config);
        }

        [Fact]
        public void CanSendMessage_PhoneNumberLimitNotExceeded_ReturnsTrue()
        {
            // Act
            var canSend1 = _rateLimiterService.CanSendMessage("12345");
            var canSend2 = _rateLimiterService.CanSendMessage("12345");

            // Assert
            Assert.True(canSend1);
            Assert.True(canSend2);
        }

        [Fact]
        public void CanSendMessage_PhoneNumberLimitExceeded_ReturnsFalse()
        {
            // Act
            _rateLimiterService.CanSendMessage("12345");
            _rateLimiterService.CanSendMessage("12345");
            var canSend3 = _rateLimiterService.CanSendMessage("12345");

            // Assert
            Assert.False(canSend3);
        }

        [Fact]
        public void CanSendMessage_AccountWideLimitNotExceeded_ReturnsTrue()
        {
            // Act
            var canSend1 = _rateLimiterService.CanSendMessage("12345");
            var canSend2 = _rateLimiterService.CanSendMessage("12346");
            var canSend3 = _rateLimiterService.CanSendMessage("12347");
            var canSend4 = _rateLimiterService.CanSendMessage("12348");
            var canSend5 = _rateLimiterService.CanSendMessage("12349");

            // Assert
            Assert.True(canSend1);
            Assert.True(canSend2);
            Assert.True(canSend3);
            Assert.True(canSend4);
            Assert.True(canSend5);
        }

        [Fact]
        public void CanSendMessage_AccountWideLimitExceeded_ReturnsFalse()
        {
            // Act
            _rateLimiterService.CanSendMessage("12345");
            _rateLimiterService.CanSendMessage("12346");
            _rateLimiterService.CanSendMessage("12347");
            _rateLimiterService.CanSendMessage("12348");
            _rateLimiterService.CanSendMessage("12349");
            var canSend6 = _rateLimiterService.CanSendMessage("12350");

            // Assert
            Assert.False(canSend6);
        }

        [Fact]
        public void CleanupInactivePhoneNumbers_RemovesInactiveNumbers()
        {
            // Arrange
            _rateLimiterService.CanSendMessage("12345");
            Thread.Sleep(TimeSpan.FromSeconds(2)); // Simulate inactivity

            // Act
            _rateLimiterService.CleanupInactivePhoneNumbers(new AutoResetEvent(false));

            // Try to add a message for "12345" again
            var canSend = _rateLimiterService.CanSendMessage("12345");

            // Assert
            Assert.True(canSend); // Expect true because the previous record for "12345" was cleaned up
        }

        [Fact]
        public void CanSendMessage_RespectsExpirationPeriod()
        {
            // Arrange
            _rateLimiterService.CanSendMessage("12345");
            _rateLimiterService.CanSendMessage("12345");

            // Wait to allow timestamps to expire
            Thread.Sleep(TimeSpan.FromSeconds(2));

            // Act
            var canSendAgain = _rateLimiterService.CanSendMessage("12345");

            // Assert
            Assert.True(canSendAgain); // Expect true because previous messages expired
        }

       
    }
}

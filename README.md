Here’s a sample README file for your GitHub repository, covering all essential details about the `MessageGatekeeper` microservice:

---

# MessageGatekeeper Microservice

## Overview

**MessageGatekeeper** is a .NET Core microservice that acts as a gatekeeper, determining whether a message can be sent from a business phone number without exceeding provider-enforced rate limits. This microservice is designed to handle high volumes of requests, ensuring efficient and reliable management of both per-phone and account-wide message rate limits.

## Features

- **Rate Limiting**: Limits messages per phone number and across the entire account per second.
- **Resource Management**: Automatically removes inactive phone numbers from tracking after a specified period.
- **Scalable Design**: Built to handle requests from multiple applications across your infrastructure.
- **Configurable Limits**: Easily customize message limits and expiration periods.

## Prerequisites

- [.NET Core 6.0 SDK](https://dotnet.microsoft.com/download)
- Optional: [Redis](https://redis.io/) or other distributed cache, if deploying in a distributed environment for scalability.

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/MessageGatekeeper.git
cd MessageGatekeeper
```

### 2. Configure Rate Limits

Edit `appsettings.json` or use environment variables to configure rate limit settings:

```json
{
  "RateLimitConfig": {
    "MaxMessagesPerPhoneNumberPerSecond": 2,
    "MaxMessagesPerAccountPerSecond": 5,
    "PhoneNumberExpirationMinutes": 5
  }
}
```

- **MaxMessagesPerPhoneNumberPerSecond**: Maximum messages allowed per phone number per second.
- **MaxMessagesPerAccountPerSecond**: Maximum messages allowed across the entire account per second.
- **PhoneNumberExpirationMinutes**: Time (in minutes) after which inactive phone numbers are removed from tracking.

### 3. Run the Application

```bash
dotnet run
```

The API should now be accessible at `http://localhost:5000`.

### 4. API Usage

#### Check if a Message Can Be Sent

**Endpoint**: `GET /api/messagegatekeeper/can-send`

**Parameters**: 
- `phoneNumber` (string): The phone number from which the message is being sent.

**Example**:
```bash
curl "http://localhost:5000/api/messagegatekeeper/can-send?phoneNumber=12345"
```

**Response**:
```json
{
  "CanSend": true
}
```

### 5. Run Tests

The solution includes unit tests for the rate limiter service, located in the `RateLimiterServiceTests` project. Run the tests with:

```bash
dotnet test
```

## Project Structure

- **Controllers**: Contains `MessageGatekeeperController` for handling API requests.
- **Services**: Contains `RateLimiterService`, implementing the rate-limiting and cleanup logic.
- **Models**: Contains `RateLimitConfig` to define configuration properties.
- **Tests**: Unit tests for `RateLimiterService`, located in the `RateLimiterServiceTests` project.

## Deployment

To deploy in production, configure the application to use a distributed cache (e.g., Redis) if multiple instances are required. Set environment variables for rate limits and any other configurations as needed.

## Contributing

Contributions are welcome! Please fork this repository, create a feature branch, and submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## Contact

For issues or inquiries, please open an issue on GitHub or reach out to the project maintainer.

---

This README provides clear instructions for setting up, configuring, and running the microservice, as well as guidelines for contributing and testing. Let me know if you need further customization!

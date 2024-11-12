using MessageGateKeeper.API.Models;
using MessageGateKeeper.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//SignalR to real time update MessageTracker
builder.Services.AddSignalR();

builder.Services.AddSingleton(new RateLimitConfig
{
    MaxMessagesPerPhoneNumberPerSecond = 1,
    MaxMessagesPerAccountPerSecond = 5
});
builder.Services.AddSingleton<RateLimiterService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

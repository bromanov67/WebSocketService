using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.OpenApi.Models;
using WebSocketService.Cache;
using WebSocketService.Infrastructure;
using WebSocketService.Models;
using WebSocketService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<ICacheService, CacheService>();



builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Message Service API", Version = "v1" });
});


//  SignalR
var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7015/messageHub")
    .Build();

connection.On<Message>("ReceiveMessage", message =>
{
    Console.WriteLine($"Received message: {message.Text} at {message.CreatedAt} with OrderNumber {message.OrderNumber}");
});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

var logger = app.Logger;

try
{
    await connection.StartAsync();
    logger.LogInformation("Connection started.");
}
catch (Exception ex)
{
    logger.LogError(ex, "Error starting SignalR connection");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Message Service API V1");
});

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<MessageHub>("/messageHub");


app.Run();

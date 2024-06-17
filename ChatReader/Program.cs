using ChatReader.Data;
using ChatReader.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<UserData>();
builder.Services.AddSingleton<IWebSocketClient, TwitchWebSocketClient>();
builder.Services.AddSingleton<WebSocketService>();

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:8080")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

var app = builder.Build();

app.UseRouting();
app.UseCors();
app.UseAuthorization();

app.UseFileServer();

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

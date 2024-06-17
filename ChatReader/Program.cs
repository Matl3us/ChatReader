using ChatReader.Data;
using ChatReader.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<UserData>();
builder.Services.AddSingleton<IWebSocketClient, TwitchWebSocketClient>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

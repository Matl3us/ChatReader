using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Net;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using ChatReader;

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
DotEnv.Load(dotenv);

string clientId = Environment.GetEnvironmentVariable("CLIENT_ID")!;
string clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET")!;

Console.WriteLine("What is your nick?");
string? nick = Console.ReadLine();
if (nick == null)
{
    Console.Error.WriteLine("Nick cannot be empty");
    return;
}

string url = "https://id.twitch.tv/oauth2/";
string responseType = "code";
string redirect_uri = "http://localhost:3000/";
string scope = "chat:read";
string requestUrl = $"{url}authorize?response_type={responseType}&client_id={clientId}&redirect_uri={redirect_uri}&scope={scope}";

Process.Start(new ProcessStartInfo(requestUrl) { UseShellExecute = true });

HttpListener httpListener = new();
httpListener.Prefixes.Add(redirect_uri);
httpListener.Start();

string? code;
HttpListenerContext context;
HttpListenerResponse response;
do
{
    context = await httpListener.GetContextAsync();
    var request = context.Request;
    code = request.QueryString.Get("code");
    response = context.Response;
    if (code == null)
    {
        response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.Close();
    }
} while (code == null);

string responseString = "Authentication completed successfully";
byte[] resBuffer = Encoding.UTF8.GetBytes(responseString);
response.ContentLength64 = resBuffer.Length;

Stream output = response.OutputStream;
output.Write(resBuffer, 0, resBuffer.Length);
output.Close();

httpListener.Stop();

HttpClient httpClient = new()
{
    BaseAddress = new Uri(url),
};
httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

var data = new Dictionary<string, string>
{
    {"client_id", clientId},
    {"client_secret", clientSecret},
    {"code", code},
    {"grant_type", "authorization_code"},
    {"redirect_uri", redirect_uri}
};
FormUrlEncodedContent form = new(data);
var res = await httpClient.PostAsync("token", form);
string msg = await res.Content.ReadAsStringAsync();
var twitchData = JsonSerializer.Deserialize<AccessTokenDto>(msg);

string? token = twitchData.access_token;
if (token == null)
{
    Console.Error.WriteLine("Invalid token");
    return;
}

Console.WriteLine("What channel do you want to join?");
string? channel = Console.ReadLine();
if (channel == null)
{
    Console.Error.WriteLine("Channel cannot be empty");
    return;
}

Console.Title = "ChatReader Client";

using (ClientWebSocket webSocket = new())
{
    try
    {
        await webSocket.ConnectAsync(new Uri("wss://irc-ws.chat.twitch.tv:443"), CancellationToken.None);
        var buffer = new byte[256];
        var sb = new StringBuilder();

        Console.WriteLine("Connection opened");
        await webSocket.SendAsync(Encoding.UTF8.GetBytes($"PASS oauth:{token}"), WebSocketMessageType.Text, true, CancellationToken.None);
        await webSocket.SendAsync(Encoding.UTF8.GetBytes($"NICK {nick}"), WebSocketMessageType.Text, true, CancellationToken.None);
        await webSocket.SendAsync(Encoding.UTF8.GetBytes($"JOIN #{channel}"), WebSocketMessageType.Text, true, CancellationToken.None);

        while (webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult? result = null;
            do
            {
                result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
            } while (!result.EndOfMessage);

            Console.Write(Parser.ParseMessage(sb.ToString()));
            Console.WriteLine("===================================================");

            sb.Clear();
        }
    }
    catch (Exception e)
    {
        Console.Error.WriteLine("Error has ocurred:\n" + e.Message);
        return;
    }
}
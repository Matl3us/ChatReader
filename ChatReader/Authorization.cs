using System.Net.Http.Headers;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Net;

class Authorization
{
    public Uri Uri { get; set; }
    public Uri RedirectUri { get; set; }
    public string Scope { get; set; }
    public string? Token { get; set; }
    private readonly HttpListener Server;
    private readonly HttpClient Client;
    private readonly string _clientId = Environment.GetEnvironmentVariable("CLIENT_ID")!;
    private readonly string _clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET")!;

    public Authorization(string url, string redirect_uri, string scope)
    {
        Uri = new Uri(url);
        RedirectUri = new Uri(redirect_uri);
        Scope = scope;

        Server = new HttpListener();
        Server.Prefixes.Add(redirect_uri);
        Server.Start();

        Client = new HttpClient()
        {
            BaseAddress = new Uri(url),
        };
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> Listen()
    {
        string? code;
        HttpListenerContext listenerContext;
        HttpListenerResponse listenerResponse;
        do
        {
            listenerContext = await Server.GetContextAsync();
            var request = listenerContext.Request;
            code = request.QueryString.Get("code");
            listenerResponse = listenerContext.Response;
            if (code == null)
            {
                listenerResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                listenerResponse.Close();
            }
        } while (code == null);

        string responseString = "Authentication completed successfully";
        byte[] resBuffer = Encoding.UTF8.GetBytes(responseString);
        listenerResponse.ContentLength64 = resBuffer.Length;

        Stream output = listenerResponse.OutputStream;
        output.Write(resBuffer, 0, resBuffer.Length);
        output.Close();
        Server.Stop();
        return code;
    }

    public async Task Authorize()
    {
        Uri requestUri = new($"{Uri}authorize?response_type=code&client_id={_clientId}&redirect_uri={RedirectUri}&scope={Scope}");
        Process.Start(new ProcessStartInfo(requestUri.ToString()) { UseShellExecute = true });

        string code = await Listen();

        var data = new Dictionary<string, string>
        {
            {"client_id", _clientId},
            {"client_secret", _clientSecret},
            {"code", code},
            {"grant_type", "authorization_code"},
            {"redirect_uri", RedirectUri.ToString()}
        };
        FormUrlEncodedContent form = new(data);
        var response = await Client.PostAsync("token", form);
        string content = await response.Content.ReadAsStringAsync();
        var twitchData = JsonSerializer.Deserialize<AccessTokenDto>(content);
        Token = twitchData.access_token;
    }
}
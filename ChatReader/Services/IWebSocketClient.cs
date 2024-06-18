namespace ChatReader.Services;

public interface IWebSocketClient
{
    public void Start(string token, CancellationToken cancellationToken);
    public Task SendMessage(string message, CancellationToken cancellationToken);
    public string? DequeueParsedMessage();
}
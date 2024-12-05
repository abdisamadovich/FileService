using System.Text.Json;
using WebTotalCommander.FileAccess.Utils;

namespace WebTotalCommander.Repository.Settings;

public class DropBoxSettings
{
    public string AppName { get; }
    public string AppKey { get; }
    public string AppSecret { get; }
    public string AuthUrl { get; }
    public string RefreshToken { get; }

    public DropBoxSettings(string appName, string appKey, string appSecret, string authUrl, string refreshToken)
    {
        AppName = appName;
        AppKey = appKey;
        AppSecret = appSecret;
        AuthUrl = authUrl;
        RefreshToken = refreshToken;
    }

    public async Task<string> GetAccessToken()
    {
        var httpClient = new HttpClient();
        var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", RefreshToken),
                new KeyValuePair<string, string>("client_id", AppKey),
                new KeyValuePair<string, string>("client_secret", AppSecret),
            }
        );

        var response = await httpClient.PostAsync("https://api.dropboxapi.com/oauth2/token", content);

        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

        var accessToken = tokenResponse.AccessToken;

        return accessToken;
    }
}

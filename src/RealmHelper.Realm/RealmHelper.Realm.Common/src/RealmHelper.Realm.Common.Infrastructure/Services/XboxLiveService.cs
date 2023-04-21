using System.Text.Json;

using RestSharp;
using RestSharp.Serializers.Json;

namespace RealmHelper.Realm.Common.Infrastructure.Services;

public interface IXboxLiveService
{
    Task<(string, string)> GetXblTokenAsync(string accessToken);

    Task<(string, string)> GetXstsTokenAsync(string xblToken, string relyingParty);
}

public sealed class XboxLiveService : IXboxLiveService, IDisposable
{
    private const string AuthenticationUrl = "https://user.auth.xboxlive.com/user/authenticate";
    private const string AuthorizationUrl = "https://xsts.auth.xboxlive.com/xsts/authorize";
    
    private readonly RestClient _restClient = new(configureSerialization: config => 
        config.UseSystemTextJson(new JsonSerializerOptions(JsonSerializerDefaults.General)));

    public async Task<(string, string)> GetXblTokenAsync(string accessToken)
    {
        var request = new RestRequest(AuthenticationUrl, Method.Post)
            .AddJsonBody(new
            {
                Properties = new
                {
                    AuthMethod = "RPS",
                    SiteName = "user.auth.xboxlive.com",
                    RpsTicket = $"d={accessToken}"
                },
                RelyingParty = "http://auth.xboxlive.com",
                TokenType = "JWT"
            });

        return ParseResponse(await _restClient.PostAsync(request));
    }

    public async Task<(string, string)> GetXstsTokenAsync(string xblToken, string relyingParty)
    {
        var request = new RestRequest(AuthorizationUrl, Method.Post)
            .AddJsonBody(new
            {
                Properties = new
                {
                    SandboxId = "RETAIL",
                    UserTokens = new [] { xblToken }
                },
                RelyingParty = relyingParty,
                TokenType = "JWT"
            });

        return ParseResponse(await _restClient.PostAsync(request));
    }

    private static (string, string) ParseResponse(RestResponse response)
    {
        var content = JsonDocument.Parse(response.Content!);
        var token = content.RootElement.GetProperty("Token").GetString();
        var userHash = content.RootElement
            .GetProperty("DisplayClaims")
            .GetProperty("xui")[0]
            .GetProperty("uhs").GetString();

        return (token!, userHash!);
    }

    public void Dispose() =>
        _restClient.Dispose();
}
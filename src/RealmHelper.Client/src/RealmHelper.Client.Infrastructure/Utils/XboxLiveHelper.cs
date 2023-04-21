using System.Text.Json;

using RestSharp;

using RealmHelper.Client.Application.Models.XboxLive.Responses;
using RestSharp.Serializers.Json;

namespace RealmHelper.Client.Infrastructure.Utils;

public static class XboxLiveHelper
{
    private static readonly RestClient RestClient = new(configureSerialization: cfg =>
        cfg.UseSystemTextJson(new JsonSerializerOptions(JsonSerializerDefaults.General)));

    private const string AuthenticationUri = "https://user.auth.xboxlive.com/user/authenticate";
    private const string AuthorizationUri = "https://xsts.auth.xboxlive.com/xsts/authorize";

    public static async Task<(string token, string userHash)> ObtainXblTokenAsync(string accessToken,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(AuthenticationUri, Method.Post)
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

        var response = await RestClient.PostAsync<AuthResponse>(request, cancellationToken);

        return (response!.Token, response.DisplayClaims.Xui[0].Uhs);
    }

    public static async Task<string> ObtainXstsTokenAsync(string xblToken, string relyingParty,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(AuthorizationUri, Method.Post)
            .AddJsonBody(new
            {
                Properties = new
                {
                    SandboxId = "RETAIL",
                    UserTokens = new[] { xblToken }
                },
                RelyingParty = relyingParty,
                TokenType = "JWT"
            });

        var response = await RestClient.PostAsync<AuthResponse>(request, cancellationToken);

        return response!.Token;
    }
}
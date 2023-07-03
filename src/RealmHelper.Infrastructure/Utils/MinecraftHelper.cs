using RestSharp;

using RealmHelper.Application.Models.Minecraft.Responses;

namespace RealmHelper.Infrastructure.Utils;

public static class MinecraftHelper
{
    private const string Endpoint = "https://api.minecraftservices.com";

    private const string AuthenticationPath = "/authentication/login_with_xbox";
    private const string ProfilePath = "/minecraft/profile";

    private static readonly RestClient RestClient = new(Endpoint);

    public static async Task<string> AuthenticateAsync(string userHash, string xstsToken,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(AuthenticationPath, Method.Post)
            .AddJsonBody(new { identityToken = $"XBL3.0 x={userHash};{xstsToken}" });

        var response = await RestClient.PostAsync<JavaAuthenticationResponse>(request, cancellationToken);

        return response!.AccessToken;
    }

    public static async Task<(string? uuid, string? username)> GetProfileAsync(string accessToken,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(ProfilePath).AddHeader(KnownHeaders.Authorization, $"Bearer {accessToken}");
        var response = await RestClient.GetAsync<JavaProfileResponse>(request, cancellationToken);

        return (response!.Id, response.Name);
    }
}
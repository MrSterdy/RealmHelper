using System.Security.Claims;

using RestSharp;

using RealmHelper.Client.Application.Models.XboxLive.Responses;
using RealmHelper.Client.Application.Repositories;
using RealmHelper.Client.Infrastructure.Authentication;
using RealmHelper.Client.Infrastructure.Http;

namespace RealmHelper.Client.Infrastructure.Repositories;

public sealed class ProfileRepository : IProfileRepository, IDisposable
{
    private const string UsersUrl = "https://profile.xboxlive.com/users";

    private const string SettingsPath = "/{PROFILE}/profile/settings";
    
    private static readonly string[] Properties = { ProfileProperty.Gamertag, ProfileProperty.GameDisplayPicRaw };
    
    private readonly IRestClient _restClient;

    public ProfileRepository(ClaimsPrincipal user)
    {
        var xstsToken = user.FindFirst(AuthClaims.XboxLiveXstsToken)!.Value;
        var userHash = user.FindFirst(AuthClaims.XboxLiveUserHash)!.Value;

        _restClient = new RestClient(UsersUrl,
                opts => opts.Authenticator = new XboxLiveAuthenticator(xstsToken, userHash))
            .AddDefaultHeader(XboxLiveHeaders.ContractVersion, "2");
    }

    public Task<ProfilesResponse> GetProfileAsync(CancellationToken cancellationToken = default) =>
        GetProfileAsyncInternal("me", cancellationToken);

    public Task<ProfilesResponse> GetProfileByXuidAsync(string xuid, CancellationToken cancellationToken = default) =>
        GetProfileAsyncInternal($"xuid({xuid})", cancellationToken);

    public Task<ProfilesResponse> GetProfileByGamertagAsync(string gamertag,
        CancellationToken cancellationToken = default) =>
        GetProfileAsyncInternal($"gt({gamertag})", cancellationToken);

    private Task<ProfilesResponse> GetProfileAsyncInternal(string profile,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(SettingsPath.Replace("{PROFILE}", profile))
            .AddQueryParameter("settings", string.Join(',', Properties));

        return _restClient.GetAsync<ProfilesResponse>(request, cancellationToken)!;
    }

    public void Dispose() =>
        _restClient.Dispose();
}
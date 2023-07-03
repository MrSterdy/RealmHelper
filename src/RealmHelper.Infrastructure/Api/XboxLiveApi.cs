using System.Security.Claims;

using RestSharp;

using RealmHelper.Application.Api;
using RealmHelper.Application.Authentication;
using RealmHelper.Application.Models.XboxLive.Responses;
using RealmHelper.Infrastructure.Authentication;
using RealmHelper.Infrastructure.Http;

namespace RealmHelper.Infrastructure.Api;

public class XboxLiveApi : IXboxLiveApi
{
    private const string ClubPresenceUrl = "https://clubhub.xboxlive.com/clubs/ids({CLUB})/decoration/clubPresence";
    
    private const string ClubActivityUrl = "https://avty.xboxlive.com/clubs/clubId({CLUB})/activity/feed?numItems={ITEMS}";
    
    private const string PeopleBatchUrl = "https://peoplehub.xboxlive.com/users/me/people/batch";
    
    private const string ProfileSettingsUrl = "https://profile.xboxlive.com/users/{PROFILE}/profile/settings";
    
    private readonly IRestClient _restClient;

    public XboxLiveApi(ClaimsPrincipal user)
    {
        var xstsToken = user.FindFirst(AuthClaims.XboxLiveXstsToken)!.Value;
        var userHash = user.FindFirst(AuthClaims.XboxLiveUserHash)!.Value;
        
        _restClient = new RestClient(opts => opts.Authenticator = new XboxLiveAuthenticator(xstsToken, userHash))
            .AddDefaultHeader(AdditionalHeaders.AcceptLanguage, "en-US");
    }
    
    public Task<ClubsResponse> GetClubAsync(long clubId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(ClubPresenceUrl.Replace("{CLUB}", clubId.ToString()))
            .AddHeader(AdditionalHeaders.ContractVersion, "5");
        
        return _restClient.GetAsync<ClubsResponse>(request, cancellationToken)!;
    }

    public Task<ClubActivitiesResponse> GetClubActivitiesAsync(long clubId, int amount,
        CancellationToken cancellationToken = default)
    {
        var request =
            new RestRequest(ClubActivityUrl.Replace("{CLUB}", clubId.ToString()).Replace("{ITEMS}", amount.ToString()))
                .AddHeader(AdditionalHeaders.ContractVersion, "13");

        return _restClient.GetAsync<ClubActivitiesResponse>(request, cancellationToken)!;
    }
    
    public Task<PeopleResponse> GetPeopleAsync(string[] xuids, CancellationToken cancellationToken = default)
    {
        if (xuids.Length == 0)
            return Task.FromResult(new PeopleResponse { People = Array.Empty<PersonResponse>() });

        var request = new RestRequest(PeopleBatchUrl, Method.Post)
            .AddJsonBody(new { xuids })
            .AddHeader(AdditionalHeaders.ContractVersion, "3");
        
        return _restClient.PostAsync<PeopleResponse>(request, cancellationToken)!;
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
        var request = new RestRequest(ProfileSettingsUrl.Replace("{PROFILE}", profile))
            .AddQueryParameter("settings",
                string.Join(',', ProfileProperty.Gamertag, ProfileProperty.GameDisplayPicRaw))
            .AddHeader(AdditionalHeaders.ContractVersion, "2");

        return _restClient.GetAsync<ProfilesResponse>(request, cancellationToken)!;
    }
}
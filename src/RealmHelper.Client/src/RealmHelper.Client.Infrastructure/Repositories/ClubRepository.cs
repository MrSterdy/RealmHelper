using System.Security.Claims;

using RestSharp;

using Microsoft.Net.Http.Headers;

using RealmHelper.Client.Application.Models.XboxLive.Responses;
using RealmHelper.Client.Application.Repositories;
using RealmHelper.Client.Infrastructure.Authentication;
using RealmHelper.Client.Infrastructure.Http;

namespace RealmHelper.Client.Infrastructure.Repositories;

public sealed class ClubRepository : IClubRepository, IDisposable
{
    private const string ClubPresenceUrl = "https://clubhub.xboxlive.com/clubs/ids({CLUB})/decoration/clubPresence";
    
    private const string ActivityUrl = "https://avty.xboxlive.com/clubs/clubId({CLUB})/activity/feed?numItems={ITEMS}";
    
    private readonly IRestClient _restClient;

    public ClubRepository(ClaimsPrincipal user)
    {
        var xstsToken = user.FindFirst(AuthClaims.XboxLiveXstsToken)!.Value;
        var userHash = user.FindFirst(AuthClaims.XboxLiveUserHash)!.Value;
        
        _restClient = new RestClient(opts => opts.Authenticator = new XboxLiveAuthenticator(xstsToken, userHash))
            .AddDefaultHeader(HeaderNames.AcceptLanguage, "en-US");
    }
    
    public Task<ClubsResponse> GetClubAsync(long clubId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(ClubPresenceUrl.Replace("{CLUB}", clubId.ToString()))
            .AddHeader(XboxLiveHeaders.ContractVersion, "5");
        
        return _restClient.GetAsync<ClubsResponse>(request, cancellationToken)!;
    }

    public Task<ClubActivitiesResponse> GetClubActivitiesAsync(long clubId, int amount,
        CancellationToken cancellationToken = default)
    {
        var request =
            new RestRequest(ActivityUrl.Replace("{CLUB}", clubId.ToString()).Replace("{ITEMS}", amount.ToString()))
                .AddHeader(XboxLiveHeaders.ContractVersion, "13");

        return _restClient.GetAsync<ClubActivitiesResponse>(request, cancellationToken)!;
    }

    public void Dispose() =>
        _restClient.Dispose();
}
using System.Security.Claims;

using Microsoft.Net.Http.Headers;

using RestSharp;

using RealmHelper.Client.Application.Models.XboxLive.Responses;
using RealmHelper.Client.Application.Repositories;
using RealmHelper.Client.Infrastructure.Authentication;
using RealmHelper.Client.Infrastructure.Http;

namespace RealmHelper.Client.Infrastructure.Repositories;

public sealed class PeopleRepository : IPeopleRepository, IDisposable
{
    private const string UsersUrl = "https://peoplehub.xboxlive.com/users";
    
    private const string BatchPath = "/me/people/batch";

    private readonly IRestClient _restClient;
    
    public PeopleRepository(ClaimsPrincipal user)
    {
        var xstsToken = user.FindFirst(AuthClaims.XboxLiveXstsToken)!.Value;
        var userHash = user.FindFirst(AuthClaims.XboxLiveUserHash)!.Value;
        _restClient = new RestClient(UsersUrl,
                opts => opts.Authenticator = new XboxLiveAuthenticator(xstsToken, userHash))
            .AddDefaultHeader(XboxLiveHeaders.ContractVersion, "3")
            .AddDefaultHeader(HeaderNames.AcceptLanguage, "en-US");
    }
    
    public Task<PeopleResponse> GetPeopleAsync(string[] xuids, CancellationToken cancellationToken = default)
    {
        if (xuids.Length == 0)
            return Task.FromResult(new PeopleResponse { People = Array.Empty<PersonResponse>() });

        var request = new RestRequest(BatchPath, Method.Post)
            .AddJsonBody(new { xuids });
        
        return _restClient.PostAsync<PeopleResponse>(request, cancellationToken)!;
    }

    public void Dispose() =>
        _restClient.Dispose();
}
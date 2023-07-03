using RestSharp;
using RestSharp.Authenticators;

namespace RealmHelper.Infrastructure.Authentication;

public class XboxLiveAuthenticator : AuthenticatorBase
{
    public XboxLiveAuthenticator(string xstsToken, string userHash) : base($"XBL3.0 x={userHash};{xstsToken}")
    {
    }

    protected override ValueTask<Parameter> GetAuthenticationParameter(string accessToken) =>
        new(new HeaderParameter(KnownHeaders.Authorization, accessToken));
}
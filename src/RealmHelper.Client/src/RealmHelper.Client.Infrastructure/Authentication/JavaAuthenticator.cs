using RestSharp;
using RestSharp.Authenticators;

namespace RealmHelper.Client.Infrastructure.Authentication;

public class JavaAuthenticator : AuthenticatorBase
{
    public JavaAuthenticator(string accessToken, string uuid, string user, string version) 
        : base($"MCJE sid=token:{accessToken}:{uuid};user={user};version={version}")
    {
    }

    protected override ValueTask<Parameter> GetAuthenticationParameter(string accessToken) =>
        new(new HeaderParameter(KnownHeaders.Authorization, accessToken));
}
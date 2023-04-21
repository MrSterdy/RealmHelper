using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace RealmHelper.Realm.Bedrock.Infrastructure.Authentication;

public class XboxLiveHandler : AuthenticationHandler<XboxLiveOptions>
{
    public XboxLiveHandler(
        IOptionsMonitor<XboxLiveOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        const string prefix = "XBL3.0 ";

        var authorization = Request.Headers[HeaderNames.Authorization].ToString();

        if (!authorization.StartsWith(prefix))
            return Task.FromResult(AuthenticateResult.NoResult());

        var split = authorization[prefix.Length..].Split(";");
        if (split.Length != 2)
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));

        var rawUserHash = split[0];
        if (!rawUserHash.StartsWith("x="))
            return Task.FromResult(AuthenticateResult.Fail("Bad User Hash"));

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(AuthClaims.UserHash, rawUserHash["x=".Length..]),
            new Claim(AuthClaims.XstsToken, split[1])
        }, XboxLiveDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        var context = new XboxLiveSuccessContext(Context, Scheme, Options)
        {
            Principal = principal
        };

        context.Success();

        return Task.FromResult(context.Result);
    }
}
using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace RealmHelper.Realm.Java.Infrastructure.Authentication;

public class MinecraftJavaHandler : AuthenticationHandler<MinecraftJavaOptions>
{
    public MinecraftJavaHandler(
        IOptionsMonitor<MinecraftJavaOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorization = Request.Headers[HeaderNames.Authorization].ToString();

        const string prefix = "MCJE ";

        if (!authorization.StartsWith(prefix))
            return Task.FromResult(AuthenticateResult.NoResult());

        var split = authorization[prefix.Length..].Split(";");
        if (split.Length != 3)
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));

        var sid = split.FirstOrDefault(f => f.StartsWith("sid="))?["sid=".Length..];
        if (sid is null) 
            return Task.FromResult(AuthenticateResult.Fail("SID Not Found"));
        
        var user = split.FirstOrDefault(f => f.StartsWith("user="))?["user=".Length..];
        if (user is null)
            return Task.FromResult(AuthenticateResult.Fail("User Not Found"));
        
        var version = split.FirstOrDefault(f => f.StartsWith("version="))?["version=".Length..];
        if (version is null)
            return Task.FromResult(AuthenticateResult.Fail("Version Not Found"));

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(AuthClaims.Sid, sid),
            new Claim(AuthClaims.User, user),
            new Claim(AuthClaims.Version, version)
        }, MinecraftJavaDefaults.AuthenticationScheme);

        var context = new MinecraftJavaSuccessContext(Context, Scheme, Options)
        {
            Principal = new ClaimsPrincipal(identity)
        };
        
        context.Success();

        return Task.FromResult(context.Result);
    }
}
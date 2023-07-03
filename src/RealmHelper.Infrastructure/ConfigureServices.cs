using System.Security.Claims;

using RealmHelper.Application.Api;
using RealmHelper.Application.Services;
using RealmHelper.Infrastructure.Api;
using RealmHelper.Infrastructure.Services;
using RealmHelper.Infrastructure.Utils;
using RealmHelper.Application.Authentication;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace RealmHelper.Infrastructure;

public static class ConfigureServices
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMicrosoftIdentityWebAppAuthentication(configuration)
            .EnableTokenAcquisitionToCallDownstreamApi(new[] { AuthScopes.XboxLiveSignIn })
            .AddInMemoryTokenCaches();
        
        services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, opts => 
            opts.Cookie.Name = $"{AuthDefaults.CookiePrefix}{AuthDefaults.AuthenticationScheme}");
        
                services.Configure<MicrosoftIdentityOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            options.Events.OnTokenValidated = async ctx =>
            {
                var tokenAcquisition = ctx.HttpContext.RequestServices.GetRequiredService<ITokenAcquisition>();
                var accessToken =
                    await tokenAcquisition.GetAccessTokenForUserAsync(
                        new[] { AuthScopes.XboxLiveSignIn }, user: ctx.Principal
                    );

                var (xblToken, userHash) = await XboxLiveHelper.ObtainXblTokenAsync(accessToken);

                var (xblXstsToken, mcbeXstsToken, mcjeXstsToken) = await (
                    XboxLiveHelper.ObtainXstsTokenAsync(xblToken, RelyingParties.XboxLive),
                    XboxLiveHelper.ObtainXstsTokenAsync(xblToken, RelyingParties.PocketRealms),
                    XboxLiveHelper.ObtainXstsTokenAsync(xblToken, RelyingParties.MinecraftServices)
                );
                
                var mcjeAccessToken = await MinecraftHelper.AuthenticateAsync(userHash, mcjeXstsToken);

                var claims = new List<Claim>
                {
                    new(AuthClaims.BedrockXstsToken, mcbeXstsToken),
                    new(AuthClaims.XboxLiveXstsToken, xblXstsToken),
                    new(AuthClaims.XboxLiveUserHash, userHash),
                    new(AuthClaims.JavaAccessToken, mcjeAccessToken)
                };
                
                var (uuid, username) = await MinecraftHelper.GetProfileAsync(mcjeAccessToken);
                if (uuid is not null && username is not null)
                {
                    claims.Add(new Claim(AuthClaims.JavaUser, username));
                    claims.Add(new Claim(AuthClaims.JavaUuid, uuid));
                }

                ctx.Principal!.AddIdentity(new ClaimsIdentity(claims));
                ctx.HttpContext.User = ctx.Principal;
                
                var xblService = ctx.HttpContext.RequestServices.GetRequiredService<IXboxLiveService>();

                var profile = await xblService.GetProfileAsync();

                var xuidClaim = new Claim(AuthClaims.BedrockUuid, profile.Xuid);
                
                ctx.Principal.AddIdentity(new ClaimsIdentity(new[] { xuidClaim }));
            }
        );
        
        services.AddMemoryCache();

        services.AddHttpContextAccessor();
        
        services.AddScoped<ClaimsPrincipal>(s => 
            s.GetRequiredService<IHttpContextAccessor>().HttpContext!.User);
        
        services.AddScoped<IXboxLiveApi, XboxLiveApi>();
        services.AddScoped<IXboxLiveService, XboxLiveService>();

        services.AddScoped<IJavaRealmApi, JavaRealmApi>();
        services.AddScoped<IJavaRealmService, JavaRealmService>();

        services.AddScoped<IBedrockRealmApi, BedrockRealmApi>();
        services.AddScoped<IBedrockRealmService, BedrockRealmService>();
    }
}
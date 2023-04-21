using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

using RealmHelper.Client.Application.Repositories;
using RealmHelper.Client.Application.Services;
using RealmHelper.Client.Infrastructure.Authentication;
using RealmHelper.Client.Infrastructure.Repositories;
using RealmHelper.Client.Infrastructure.Services;
using RealmHelper.Client.Infrastructure.Utils;
using RealmHelper.Realm.Bedrock.Abstractions.Services;
using RealmHelper.Realm.Java.Abstractions.Services;

namespace RealmHelper.Client.Infrastructure;

public static class ConfigureServices
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMicrosoftIdentityWebAppAuthentication(configuration)
            .EnableTokenAcquisitionToCallDownstreamApi(new[] { AuthScopes.XboxLiveSignIn })
            .AddInMemoryTokenCaches();

        services.Configure<WebApiOptions>(configuration.GetSection(WebApiOptions.Section));
        
        services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, opts => 
            opts.Cookie.Name = $"{AuthDefaults.Prefix}{AuthDefaults.Scheme}");
        
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
                
                var profileService = ctx.HttpContext.RequestServices.GetRequiredService<IProfileService>();

                var profile = await profileService.GetProfileAsync();

                var xuidClaim = new Claim(AuthClaims.BedrockUuid, profile.Xuid);
                
                ctx.Principal.AddIdentity(new ClaimsIdentity(new[] { xuidClaim }));
            }
        );

        services.AddScoped<ClaimsPrincipal>(s => 
            s.GetRequiredService<IHttpContextAccessor>().HttpContext!.User);

        services.AddScoped<IClubRepository, ClubRepository>();
        services.AddScoped<IPeopleRepository, PeopleRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();

        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IPeopleService, PeopleService>();
        services.AddScoped<IClubService, ClubService>();
        services.AddScoped<IJavaRealmService, JavaRealmService>();
        services.AddScoped<IBedrockRealmService, BedrockRealmService>();
    }
}
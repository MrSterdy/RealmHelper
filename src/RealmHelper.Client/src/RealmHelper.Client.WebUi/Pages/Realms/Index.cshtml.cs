using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

using RealmHelper.Client.Infrastructure.Authentication;
using RealmHelper.Client.Infrastructure.Utils;
using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Bedrock.Abstractions.Services;
using RealmHelper.Realm.Java.Abstractions.Models;
using RealmHelper.Realm.Java.Abstractions.Services;

namespace RealmHelper.Client.WebUi.Pages.Realms;

[Authorize]
[AuthorizeForScopes]
public class Index : PageModel
{
    private readonly IBedrockRealmService _bedrockService;
    private readonly IJavaRealmService _javaService;

    private readonly ClaimsPrincipal _user;

    public List<BedrockRealm> OwnedBedrockRealms { get; set; } = new();
    public List<BedrockRealm> GuestBedrockRealms { get; set; } = new();

    public List<JavaRealm> GuestJavaRealms { get; set; } = new();
    public List<JavaRealm> OwnedJavaRealms { get; set; } = new();

    public Index(IBedrockRealmService bedrockService, IJavaRealmService javaService, ClaimsPrincipal user) =>
        (_bedrockService, _javaService, _user) = (bedrockService, javaService, user);

    public async Task OnGet(CancellationToken cancellationToken)
    {
        var bedrockRealmsTask = _bedrockService.GetRealmsAsync(cancellationToken);
        var javaRealmsTask = _user.HasClaim(c => c.Type == AuthClaims.JavaUser)
            ? _javaService.GetRealmsAsync(cancellationToken)
            : Task.FromResult(Array.Empty<JavaRealm>());

        var tasks = new Task[] { bedrockRealmsTask, javaRealmsTask };

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception)
        {
            // ignored
        }

        var bedrockRealms = tasks[0].IsCompletedSuccessfully
            ? ((Task<BedrockRealm[]>)tasks[0]).Result
            : Array.Empty<BedrockRealm>();
        var javaRealms = tasks[1].IsCompletedSuccessfully
            ? ((Task<JavaRealm[]>)tasks[1]).Result
            : Array.Empty<JavaRealm>();

        foreach (var bedrockRealm in bedrockRealms)
            (bedrockRealm.IsOwner(_user) ? OwnedBedrockRealms : GuestBedrockRealms).Add(bedrockRealm);
        
        foreach (var javaRealm in javaRealms)
            (javaRealm.IsOwner(_user) ? OwnedJavaRealms : GuestJavaRealms).Add(javaRealm);

        OwnedBedrockRealms.Sort((a, _) => a.Expired ? 1 : -1);
        GuestBedrockRealms.Sort((a, _) => a.Expired ? 1 : -1);
        
        OwnedJavaRealms.Sort((a, _) => a.Expired ? 1 : -1);
        GuestJavaRealms.Sort((a, _) => a.Expired ? 1 : -1);
    }
}
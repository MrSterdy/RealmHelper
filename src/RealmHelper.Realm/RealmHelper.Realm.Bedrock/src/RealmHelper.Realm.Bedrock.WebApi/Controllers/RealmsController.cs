using Microsoft.AspNetCore.Mvc;

using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Bedrock.Abstractions.Services;
using RealmHelper.Realm.Common.WebApi.Controllers;

namespace RealmHelper.Realm.Bedrock.WebApi.Controllers;

public class RealmsController : RealmsController<BedrockRealm>
{
    private readonly IBedrockRealmService _realmService;

    public RealmsController(IBedrockRealmService realmService) =>
        _realmService = realmService;
    
    public override async Task<ActionResult<BedrockRealm[]>> GetRealms(CancellationToken cancellationToken) =>
        await _realmService.GetRealmsAsync(cancellationToken);
}
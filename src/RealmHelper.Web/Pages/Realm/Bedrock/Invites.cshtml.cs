using Microsoft.AspNetCore.Mvc;

using RealmHelper.Application.Services;
using RealmHelper.Domain.Models.XboxLive;
using RealmHelper.Web.Models;
using RealmHelper.Web.Utils;

namespace RealmHelper.Web.Pages.Realm.Bedrock;

public class Invites : OwnedBedrockRealmModel
{
    private readonly IXboxLiveService _xblService;

    [FromQuery(Name = "Page")]
    public int MembersPage { get; set; } = 1;
    
    public Profile[] Profiles { get; set; } = default!;

    public Page CurrentPage { get; set; } = default!;

    public Invites(IXboxLiveService xblService, IBedrockRealmService realmService) : base(realmService) =>
        _xblService = xblService;

    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        var result = await base.OnGet(realmId, cancellationToken);

        var players = Realm.Players!.Length;
        
        var blacklist = await RealmService.GetBlockedPlayersAsync(realmId, cancellationToken);

        var (toSkipInvites, _, _) = PaginationHelper.GetIndexes(MembersPage, players);

        var foundInvites = 0;

        var start = -1;

        for (var i = 0; i < players; i++)
        {
            if (foundInvites == toSkipInvites)
            {
                start = i;

                break;
            }
            
            var player = Realm.Players[i];

            if (!player.Accepted && !blacklist.Contains(player.Uuid))
                foundInvites++;
        }

        var end = Math.Min(players, start + Constants.PageSize);

        var pageSize = end - start;

        var xuids = new List<string>(pageSize);
        for (int i = start, j = 0; i < players; i++)
        {
            var player = Realm.Players[i];
            if (!player.Accepted && !blacklist.Contains(player.Uuid))
            {
                if (j < pageSize)
                {
                    xuids.Add(player.Uuid);
                    j++;
                }
                
                foundInvites++;
            }
        }

        Profiles = xuids.Count == 0 
            ? Array.Empty<Profile>() 
            : await _xblService.GetProfilesAsync(xuids.ToArray(), cancellationToken);

        CurrentPage = new Page
        {
            Number = PaginationHelper.GetCurrentPage(end),
            Last = PaginationHelper.IsLastPage(toSkipInvites + Constants.PageSize, foundInvites)
        };

        return result;
    }
}
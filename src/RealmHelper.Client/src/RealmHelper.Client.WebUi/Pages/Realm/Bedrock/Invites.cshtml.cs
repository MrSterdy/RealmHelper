using Microsoft.AspNetCore.Mvc;

using RealmHelper.Client.Application.Services;
using RealmHelper.Client.Domain.Models.XboxLive;
using RealmHelper.Client.WebUi.Models;
using RealmHelper.Client.WebUi.Utils;
using RealmHelper.Realm.Bedrock.Abstractions.Services;

namespace RealmHelper.Client.WebUi.Pages.Realm.Bedrock;

public class Invites : OwnedBedrockRealmModel
{
    private readonly IPeopleService _peopleService;

    [FromQuery(Name = "Page")]
    public int MembersPage { get; set; } = 1;
    
    public Profile[] Profiles { get; set; } = default!;

    public Page CurrentPage { get; set; } = default!;

    public Invites(IPeopleService peopleService, IBedrockRealmService realmService, IClubService clubService)
        : base(realmService, clubService) =>
        _peopleService = peopleService;

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
            : await _peopleService.GetProfilesAsync(xuids.ToArray(), cancellationToken);

        CurrentPage = new Page
        {
            Number = PaginationHelper.GetCurrentPage(end),
            Last = PaginationHelper.IsLastPage(toSkipInvites + Constants.PageSize, foundInvites)
        };

        return result;
    }
}
using Microsoft.AspNetCore.Mvc;

using RealmHelper.Client.WebUi.Models;
using RealmHelper.Client.WebUi.Utils;
using RealmHelper.Realm.Java.Abstractions.Models;
using RealmHelper.Realm.Java.Abstractions.Services;

namespace RealmHelper.Client.WebUi.Pages.Realm.Java;

public class Invites : OwnedJavaRealmModel
{
    [FromQuery(Name = "Page")]
    public int MembersPage { get; set; } = 1;
    
    public JavaPlayer[] Players { get; set; } = default!;

    public Page CurrentPage { get; set; } = default!;

    public Invites(IJavaRealmService realmService) : base(realmService)
    {
    }

    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        var result = await base.OnGet(realmId, cancellationToken);

        var playersLength = Realm.Players!.Length;

        var (toSkipInvites, _, _) = PaginationHelper.GetIndexes(MembersPage, playersLength);

        var foundInvites = 0;

        var start = -1;

        for (var i = 0; i < playersLength; i++)
        {
            if (foundInvites == toSkipInvites)
            {
                start = i;

                break;
            }
            
            var player = Realm.Players[i];

            if (!player.Accepted)
                foundInvites++;
        }

        var end = Math.Min(playersLength, start + Constants.PageSize);

        var pageSize = end - start;

        var players = new List<JavaPlayer>(pageSize);
        for (int i = start, j = 0; i < playersLength; i++)
        {
            var player = Realm.Players[i];
            if (player.Accepted)
                continue;
            
            if (j < pageSize)
            {
                players.Add(player);
                
                j++;
            }
                
            foundInvites++;
        }

        Players = players.ToArray();

        CurrentPage = new Page
        {
            Number = PaginationHelper.GetCurrentPage(end),
            Last = PaginationHelper.IsLastPage(toSkipInvites + Constants.PageSize, foundInvites)
        };

        return result;
    }
}
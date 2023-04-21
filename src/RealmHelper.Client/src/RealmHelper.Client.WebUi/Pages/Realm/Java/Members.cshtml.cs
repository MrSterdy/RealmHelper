using Microsoft.AspNetCore.Mvc;

using RealmHelper.Client.Infrastructure.Utils;
using RealmHelper.Client.WebUi.Models;
using RealmHelper.Client.WebUi.Utils;
using RealmHelper.Realm.Java.Abstractions.Models;
using RealmHelper.Realm.Java.Abstractions.Services;

namespace RealmHelper.Client.WebUi.Pages.Realm.Java;

public class Members : JavaRealmModel
{
    [FromQuery(Name = "Page")] 
    public int MembersPage { get; set; } = 1;

    public JavaPlayer[] OnlinePlayers { get; set; } = default!;
    public JavaPlayer[] OfflinePlayers { get; set; } = default!;

    public Page CurrentPage { get; set; } = default!;

    public Members(IJavaRealmService realmService) : base(realmService)
    {
    }

    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        await base.OnGet(realmId, cancellationToken);

        var players = Realm.IsOwner(User)
            ? Realm.Players!
            : (from p in await RealmService.GetOnlinePlayersAsync(realmId, cancellationToken)
                select new JavaPlayer { Name = p, Online = true }).ToArray();

        var (start, end, pageSize) = PaginationHelper.GetIndexes(MembersPage, players.Length);

        var page = PaginationHelper.GetCurrentPage(end);

        if (page > 1)
        {
            OnlinePlayers = Array.Empty<JavaPlayer>();
            OfflinePlayers = new JavaPlayer[pageSize];
            for (int i = start, j = 0; i < end; i++, j++)
                OfflinePlayers[j] = players[i];
        }
        else
        {
            var onlineLength = Math.Min(players.Count(p => p.Online), pageSize);

            OnlinePlayers = new JavaPlayer[onlineLength];
            OfflinePlayers = onlineLength == pageSize
                ? Array.Empty<JavaPlayer>()
                : new JavaPlayer[Math.Min(players.Length, pageSize - onlineLength)];
            for (int i = start, j = 0; i < end; i++, j++)
            {
                var player = players[j];

                if (player.Online)
                    OnlinePlayers[j] = player;
                else
                    OfflinePlayers[j - onlineLength] = player;
            }
        }

        CurrentPage = new Page
        {
            Number = page,
            Last = PaginationHelper.IsLastPage(end, players.Length)
        };

        return Page();
    }
}
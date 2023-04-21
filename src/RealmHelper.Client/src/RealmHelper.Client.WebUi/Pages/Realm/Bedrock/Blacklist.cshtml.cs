using Microsoft.AspNetCore.Mvc;

using RealmHelper.Client.Application.Services;
using RealmHelper.Client.Domain.Models.XboxLive;
using RealmHelper.Client.WebUi.Models;
using RealmHelper.Client.WebUi.Utils;
using RealmHelper.Realm.Bedrock.Abstractions.Services;

namespace RealmHelper.Client.WebUi.Pages.Realm.Bedrock;

public class Blacklist : OwnedBedrockRealmModel
{
    private readonly IPeopleService _peopleService;

    [FromQuery(Name = "Page")]
    public int MembersPage { get; set; } = 1;
    
    public Profile[] Players { get; set; } = default!;

    public Page? CurrentPage { get; set; }

    public Blacklist(IPeopleService peopleService, IBedrockRealmService realmService, IClubService clubService)
        : base(realmService, clubService) =>
        _peopleService = peopleService;

    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        await base.OnGet(realmId, cancellationToken);

        var response = await RealmService.GetBlockedPlayersAsync(realmId, cancellationToken);

        if (response.Length == 0)
        {
            Players = Array.Empty<Profile>();

            return Page();
        }

        var (start, end, pageSize) = PaginationHelper.GetIndexes(MembersPage, response.Length);

        var xuids = new string[pageSize];
        for (int i = start, j = 0; i < end; i++, j++)
            xuids[j] = response[i];

        Players = await _peopleService.GetProfilesAsync(xuids, cancellationToken);

        CurrentPage = new Page
        {
            Number = PaginationHelper.GetCurrentPage(end),
            Last = PaginationHelper.IsLastPage(end, response.Length)
        };

        return Page();
    }
}
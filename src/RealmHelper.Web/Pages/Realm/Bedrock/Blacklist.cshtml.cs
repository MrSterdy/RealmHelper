﻿using Microsoft.AspNetCore.Mvc;

using RealmHelper.Application.Services;
using RealmHelper.Domain.Models.XboxLive;
using RealmHelper.Web.Models;
using RealmHelper.Web.Utils;

namespace RealmHelper.Web.Pages.Realm.Bedrock;

public class Blacklist : OwnedBedrockRealmModel
{
    private readonly IXboxLiveService _xblService;

    [FromQuery(Name = "Page")]
    public int MembersPage { get; set; } = 1;
    
    public Profile[] Players { get; set; } = default!;

    public Page? CurrentPage { get; set; }

    public Blacklist(IXboxLiveService xblService, IBedrockRealmService realmService) : base(realmService) =>
        _xblService = xblService;

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

        Players = await _xblService.GetProfilesAsync(xuids, cancellationToken);

        CurrentPage = new Page
        {
            Number = PaginationHelper.GetCurrentPage(end),
            Last = PaginationHelper.IsLastPage(end, response.Length)
        };

        return Page();
    }
}
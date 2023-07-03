using Microsoft.AspNetCore.Mvc;

using RealmHelper.Application.Services;
using RealmHelper.Domain.Models.Minecraft.Bedrock;
using RealmHelper.Domain.Models.XboxLive;
using RealmHelper.Web.Models;
using RealmHelper.Web.Utils;

namespace RealmHelper.Web.Pages.Realm.Bedrock;

public class Members : BedrockRealmModel
{
    private readonly IXboxLiveService _xblService;

    [FromQuery(Name = "Page")] 
    public int MembersPage { get; set; } = 1;

    public (Profile, BedrockPlayer)[] OnlinePlayers { get; set; } = default!;
    public (Profile, ClubMember)[] OfflinePlayers { get; set; } = default!;

    public Page CurrentPage { get; set; } = default!;

    public Members(IXboxLiveService xblService, IBedrockRealmService realmService) : base(realmService) =>
        _xblService = xblService;

    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        await base.OnGet(realmId, cancellationToken);

        var club = await _xblService.GetClubAsync(Realm.ClubId, cancellationToken);

        var members = club.Members.Length;

        var (start, end, pageSize) = PaginationHelper.GetIndexes(MembersPage, members);

        var xuids = new string[pageSize];
        for (var i = start; i < end; i++)
            xuids[i - start] = club.Members[i].Xuid;

        var profilesTask = _xblService.GetProfilesAsync(xuids, cancellationToken);

        var page = PaginationHelper.GetCurrentPage(end);

        if (page > 1)
        {
            var profiles = await profilesTask;

            OnlinePlayers = Array.Empty<(Profile, BedrockPlayer)>();
            OfflinePlayers = new (Profile, ClubMember)[pageSize];
            for (int i = start, j = 0; i < end; i++, j++)
                OfflinePlayers[j] = (profiles[j], club.Members[i]);
        }
        else
        {
            var activitiesTask = RealmService.GetOnlinePlayersAsync(Realm.Id, cancellationToken);

            await Task.WhenAll(profilesTask, activitiesTask);

            var profiles = await profilesTask;

            var activity = await activitiesTask;
            var onlineLength = Math.Min(activity.Length, pageSize); // Sometimes offline players are returned, resulting in overflow

            OnlinePlayers = new (Profile, BedrockPlayer)[onlineLength];
            OfflinePlayers = onlineLength == pageSize
                ? Array.Empty<(Profile, ClubMember)>()
                : new (Profile, ClubMember)[Math.Min(members, pageSize - onlineLength)];
            for (int i = start, j = 0; i < end; i++, j++)
            {
                var presence = club.Members[i];
                var profile = profiles[j];
                var player = j < onlineLength ? activity[j] : null;
        
                if (player is not null)
                    OnlinePlayers[j] = (profile, player);
                else
                    OfflinePlayers[j - onlineLength] = (profile, presence);
            }
        }

        CurrentPage = new Page
        {
            Number = page,
            Last = PaginationHelper.IsLastPage(end, members)
        };

        return Page();
    }
}
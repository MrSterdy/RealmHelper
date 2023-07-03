using Microsoft.AspNetCore.Mvc;

using RealmHelper.Application.Services;
using RealmHelper.Domain.Models.Minecraft;
using RealmHelper.Web.Models;
using RealmHelper.Web.Utils;

namespace RealmHelper.Web.Pages.Realm.Bedrock;

public class Backups : OwnedBedrockRealmModel
{
    [FromQuery(Name = "Page")] 
    public int BackupsPage { get; set; } = 1;

    public Backup[] BackupArray { get; set; } = default!;
    
    public Page? CurrentPage { get; set; }

    public Backups(IBedrockRealmService realmService) : base(realmService)
    {
    }

    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        await base.OnGet(realmId, cancellationToken);

        var backups = await RealmService.GetBackupsAsync(realmId, cancellationToken);

        if (backups.Length == 0)
            return Page();

        var (start, end, pageSize) = PaginationHelper.GetIndexes(BackupsPage, backups.Length);

        BackupArray = new Backup[pageSize];
        for (var i = start; i < end; i++)
            BackupArray[i] = backups[i];

        CurrentPage = new Page
        {
            Number = PaginationHelper.GetCurrentPage(end),
            Last = PaginationHelper.IsLastPage(end, backups.Length)
        };

        return Page();
    }
}
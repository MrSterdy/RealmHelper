using System.Net;

using Microsoft.AspNetCore.Mvc;

using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Common.Application.Models.Requests;
using RealmHelper.Realm.Common.WebApi.Controllers;
using RealmHelper.Realm.Java.Abstractions.Models;
using RealmHelper.Realm.Java.Abstractions.Services;

namespace RealmHelper.Realm.Java.WebApi.Controllers;

public class RealmController : RealmController<JavaRealm, string, SlotOptionsDto>
{
    private readonly IJavaRealmService _realmService;

    public RealmController(IJavaRealmService realmService) =>
        _realmService = realmService;
    
    public override async Task<ActionResult<JavaRealm>> GetRealm(long realmId, CancellationToken cancellationToken) =>
        await _realmService.GetRealmAsync(realmId, cancellationToken);

    public override async Task<ActionResult<string[]>> GetOnlinePlayers(long realmId,
        CancellationToken cancellationToken) =>
        await _realmService.GetOnlinePlayersAsync(realmId, cancellationToken);

    public override async Task<ActionResult<Backup[]>> GetBackups(long realmId, CancellationToken cancellationToken) =>
        await _realmService.GetBackupsAsync(realmId, cancellationToken);

    public override async Task<IActionResult> DownloadBackup(long realmId, int slotId,
        CancellationToken cancellationToken, string? backupId = null)
    {
        var file = await _realmService.DownloadBackupAsync(realmId, slotId, backupId, cancellationToken);

        return File(file.Content, file.ContentType, file.FileName);
    }

    public override async Task<IActionResult> UploadBackup(long realmId, int slotId,
        CancellationToken cancellationToken, string? backupId = null)
    {
        if (backupId is not null)
        {
            try
            {
                await _realmService.RestoreBackupAsync(realmId, backupId, cancellationToken);
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                await Task.Delay(
                    1000); // Sometimes you have to call this request twice for it to function; the first request always seems to be a 503 error

                await _realmService.RestoreBackupAsync(realmId, backupId, cancellationToken);
            }

            return NoContent();
        }

        if (!Request.HasFormContentType || Request.Form.Count > 1)
            return BadRequest();

        var file = Request.Form.Files[0];
        var archiveData = new BackupFile
        {
            Name = file.Name,
            FileName = file.FileName,
            Content = file.OpenReadStream(),
            ContentType = file.ContentType
        };

        try
        {
            await _realmService.UploadBackupAsync(realmId, slotId, archiveData, cancellationToken);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }

        return NoContent();
    }

    public override async Task<IActionResult> CloseRealm(long realmId, CancellationToken cancellationToken)
    {
        await _realmService.CloseRealmAsync(realmId, cancellationToken);

        return NoContent();
    }

    public override async Task<IActionResult> OpenRealm(long realmId, CancellationToken cancellationToken)
    {
        await _realmService.OpenRealmAsync(realmId, cancellationToken);

        return NoContent();
    }

    public override async Task<IActionResult> ResetRealm(long realmId, CancellationToken cancellationToken)
    {
        await _realmService.ResetRealmAsync(realmId, cancellationToken);

        return NoContent();
    }

    public override async Task<IActionResult> InvitePlayer(long realmId, string player,
        CancellationToken cancellationToken)
    {
        await _realmService.InvitePlayerAsync(realmId, player, cancellationToken);

        return NoContent();
    }

    public override async Task<IActionResult> UninvitePlayer(long realmId, string player,
        CancellationToken cancellationToken)
    {
        await _realmService.UninvitePlayerAsync(realmId, player, cancellationToken);

        return NoContent();
    }

    public override async Task<IActionResult> UpdateDescription(long realmId, DescriptionUpdateRequest request,
        CancellationToken cancellationToken)
    {
        await _realmService.UpdateDescriptionAsync(realmId, request.Name, request.Description, cancellationToken);

        return NoContent();
    }

    public override async Task<IActionResult> UpdateSlot(long realmId, int slotId,
        SlotUpdateRequest<SlotOptionsDto> request, CancellationToken cancellationToken)
    {
        await _realmService.UpdateBulkAsync(realmId, slotId, request.Description.Name, request.Description.Description,
            request.Options, cancellationToken);

        return NoContent();
    }

    public override async Task<IActionResult> SwitchSlot(long realmId, int slotId, CancellationToken cancellationToken)
    {
        await _realmService.SwitchSlotAsync(realmId, slotId, cancellationToken);

        return NoContent();
    }

    [HttpPut("Slot/{slotId:int}/Options")]
    public async Task<IActionResult> UpdateSlotOptions(long realmId, int slotId, [FromBody] SlotOptionsDto request,
        CancellationToken cancellationToken)
    {
        await _realmService.UpdateSlotOptionsAsync(realmId, slotId, request, cancellationToken);

        return NoContent();
    }

    [HttpPost("Ops/{player}")]
    public async Task<IActionResult> Op(long realmId, string player, CancellationToken cancellationToken)
    {
        await _realmService.OpAsync(realmId, player, cancellationToken);

        return NoContent();
    }
    
    [HttpDelete("Ops/{player}")]
    public async Task<IActionResult> Deop(long realmId, string player, CancellationToken cancellationToken)
    {
        await _realmService.DeopAsync(realmId, player, cancellationToken);

        return NoContent();
    }
}
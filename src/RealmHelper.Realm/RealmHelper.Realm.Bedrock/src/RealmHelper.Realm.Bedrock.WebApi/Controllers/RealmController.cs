using System.Net;

using Microsoft.AspNetCore.Mvc;

using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Bedrock.Abstractions.Services;
using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Common.Application.Models.Requests;
using RealmHelper.Realm.Common.WebApi.Controllers;

namespace RealmHelper.Realm.Bedrock.WebApi.Controllers;

public class RealmController : RealmController<BedrockRealm, BedrockPlayer, BedrockSlotOptionsDto>
{
    private readonly IBedrockRealmService _realmService;

    public RealmController(IBedrockRealmService realmService) =>
        _realmService = realmService;

    public override async Task<ActionResult<BedrockRealm>> GetRealm(long realmId, CancellationToken cancellationToken) =>
        await _realmService.GetRealmAsync(realmId, cancellationToken);

    public override async Task<ActionResult<BedrockPlayer[]>> GetOnlinePlayers(long realmId,
        CancellationToken cancellationToken) =>
        await _realmService.GetOnlinePlayersAsync(realmId, cancellationToken);

    public override async Task<ActionResult<Backup[]>> GetBackups(long realmId, CancellationToken cancellationToken) =>
        await _realmService.GetBackupsAsync(realmId, cancellationToken);

    public override async Task<IActionResult> DownloadBackup(long realmId, int slotId,
        CancellationToken cancellationToken, string? backupId = null)
    {
        var data = await _realmService.DownloadBackupAsync(realmId, slotId, backupId, cancellationToken);

        return File(data.Content, data.ContentType, data.FileName);
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
                await Task.Delay(1000, cancellationToken); // Sometimes you have to call this request twice for it to function; the first request always seems to be a 503 error

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
        SlotUpdateRequest<BedrockSlotOptionsDto> request, CancellationToken cancellationToken)
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

    [HttpGet("Blocklist")]
    public async Task<ActionResult<string[]>> GetBlockedPlayers(long realmId, CancellationToken cancellationToken) =>
        await _realmService.GetBlockedPlayersAsync(realmId, cancellationToken);

    [HttpPost("Blocklist/{xuid}")]
    public async Task<IActionResult> BlockPlayer(long realmId, string xuid, CancellationToken cancellationToken)
    {
        await _realmService.BlockPlayerAsync(realmId, xuid, cancellationToken);

        return NoContent();
    }
    
    [HttpDelete("Blocklist/{xuid}")]
    public async Task<IActionResult> UnblockPlayer(long realmId, string xuid, CancellationToken cancellationToken)
    {
        await _realmService.UnblockPlayerAsync(realmId, xuid, cancellationToken);

        return NoContent();
    }

    [HttpPut("Permission/Player/{xuid}")]
    public async Task<IActionResult> UpdatePlayerPermission(long realmId, string xuid, [FromBody] string permission,
        CancellationToken cancellationToken)
    {
        await _realmService.ChangePlayerPermissionAsync(realmId, xuid, permission, cancellationToken);

        return NoContent();
    }

    [HttpPut("Permission/Default")]
    public async Task<IActionResult> UpdateDefaultPermission(long realmId, [FromBody] string permission,
        CancellationToken cancellationToken)
    {
        await _realmService.ChangeDefaultPermissionAsync(realmId, permission, cancellationToken);

        return NoContent();
    }

    [HttpGet("Codes/Invite")]
    public async Task<IActionResult> GetInviteCode(long realmId, CancellationToken cancellationToken) =>
        new JsonResult(await _realmService.GetInviteCodeAsync(realmId, cancellationToken));

    [HttpPost("Codes/Invite")]
    public async Task<IActionResult> GenerateInviteCode(long realmId, CancellationToken cancellationToken) =>
        new JsonResult(await _realmService.GenerateInviteCodeAsync(realmId, cancellationToken));
}
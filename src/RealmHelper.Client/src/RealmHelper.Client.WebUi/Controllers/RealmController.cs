using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

using RealmHelper.Client.Application.Models.Minecraft.Requests;
using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Bedrock.Abstractions.Services;
using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Java.Abstractions.Services;

namespace RealmHelper.Client.WebUi.Controllers;

[Authorize]
[ApiController]
[AuthorizeForScopes]
public abstract class RealmController<TSlotOptions> : ControllerBase where TSlotOptions : SlotOptionsDto
{
    [HttpGet("Slot/{slotId:int}/Backup/{backupId?}")]
    public abstract Task<IActionResult> DownloadBackup(long realmId, int slotId, string? backupId = default);

    [HttpPost("Slot/{slotId:int}/Backup/{backupId?}")]
    public abstract Task<IActionResult> UploadBackup(long realmId, int slotId, string? backupId = default);

    [HttpPatch("Slot/{slotId:int}")]
    public abstract Task<IActionResult> UpdateSlot(long realmId, int slotId,
        [FromBody] SlotUpdateRequest<TSlotOptions> request);

    [HttpPut("Description")]
    public abstract Task<IActionResult> UpdateDescription(long realmId, [FromBody] DescriptionUpdateRequest request);

    [HttpPut("Slot/{slotId:int}")]
    public abstract Task<IActionResult> ActivateSlot(long realmId, int slotId);

    [HttpPost("Open")]
    public abstract Task<IActionResult> Open(long realmId);
    
    [HttpPost("Close")]
    public abstract Task<IActionResult> Close(long realmId);
    
    [HttpPost("Reset")]
    public abstract Task<IActionResult> Reset(long realmId);

    [HttpPost("Invites/{player}")]
    public abstract Task<IActionResult> Invite(long realmId, string player);

    [HttpDelete("Invites/{player}")]
    public abstract Task<IActionResult> Uninvite(long realmId, string player);
}

[Route("Api/Java/Realm/{realmId:long}")]
public class JavaRealmController : RealmController<SlotOptionsDto>
{
    private readonly IJavaRealmService _realmService;

    public JavaRealmController(IJavaRealmService realmService) =>
        _realmService = realmService;
    
    public override async Task<IActionResult> DownloadBackup(long realmId, int slotId, string? backupId = default)
    {
        var data = await _realmService.DownloadBackupAsync(realmId, slotId, backupId);

        return File(data.Content, data.ContentType, data.FileName);
    }

    public override async Task<IActionResult> UploadBackup(long realmId, int slotId, string? backupId = default)
    {
        if (backupId is not null)
        {
            await _realmService.RestoreBackupAsync(realmId, backupId);

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

        await _realmService.UploadBackupAsync(realmId, slotId, archiveData);

        return NoContent();
    }

    public override async Task<IActionResult> UpdateSlot(long realmId, int slotId,
        SlotUpdateRequest<SlotOptionsDto> request)
    {
        await _realmService.UpdateBulkAsync(realmId, slotId, request.Description.Name, request.Description.Description,
            request.Options);

        return NoContent();
    }

    [HttpPut("Slot/{slotId:int}/Options")]
    public async Task<IActionResult> UpdateSlotOptions(long realmId, int slotId, [FromBody] SlotOptionsDto request)
    {
        await _realmService.UpdateSlotOptionsAsync(realmId, slotId, request);

        return NoContent();
    }

    public override async Task<IActionResult> UpdateDescription(long realmId, DescriptionUpdateRequest request)
    {
        await _realmService.UpdateDescriptionAsync(realmId, request.Name, request.Description);

        return NoContent();
    }

    public override async Task<IActionResult> ActivateSlot(long realmId, int slotId)
    {
        await _realmService.SwitchSlotAsync(realmId, slotId);

        return NoContent();
    }

    public override async Task<IActionResult> Open(long realmId)
    {
        await _realmService.OpenRealmAsync(realmId);

        return NoContent();
    }

    public override async Task<IActionResult> Close(long realmId)
    {
        await _realmService.CloseRealmAsync(realmId);

        return NoContent();
    }

    public override async Task<IActionResult> Reset(long realmId)
    {
        await _realmService.ResetRealmAsync(realmId);

        return NoContent();
    }

    public override async Task<IActionResult> Invite(long realmId, string player)
    {
        await _realmService.InvitePlayerAsync(realmId, player);

        return NoContent();
    }

    public override async Task<IActionResult> Uninvite(long realmId, string player)
    {
        await _realmService.UninvitePlayerAsync(realmId, player);

        return NoContent();
    }
    
    [HttpPost("Ops/{player}")]
    public async Task<IActionResult> Op(long realmId, string player)
    {
        await _realmService.OpAsync(realmId, player);

        return NoContent();
    }

    [HttpDelete("Ops/{player}")]
    public async Task<IActionResult> Deop(long realmId, string player)
    {
        await _realmService.DeopAsync(realmId, player);

        return NoContent();
    }
}

[Route("Api/Bedrock/Realm/{realmId:long}")]
public class BedrockRealmController : RealmController<BedrockSlotOptionsDto>
{
    private readonly IBedrockRealmService _realmService;

    public BedrockRealmController(IBedrockRealmService realmService) =>
        _realmService = realmService;
    
    public override async Task<IActionResult> DownloadBackup(long realmId, int slotId, string? backupId = default)
    {
        var data = await _realmService.DownloadBackupAsync(realmId, slotId, backupId);

        return File(data.Content, data.ContentType, data.FileName);
    }

    public override async Task<IActionResult> UploadBackup(long realmId, int slotId, string? backupId = default)
    {
        if (backupId is not null)
        {
            await _realmService.RestoreBackupAsync(realmId, backupId);

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

        await _realmService.UploadBackupAsync(realmId, slotId, archiveData);

        return NoContent();
    }

    public override async Task<IActionResult> UpdateSlot(long realmId, int slotId,
        SlotUpdateRequest<BedrockSlotOptionsDto> request)
    {
        await _realmService.UpdateBulkAsync(realmId, slotId, request.Description.Name, request.Description.Description,
            request.Options);

        return NoContent();
    }

    public override async Task<IActionResult> UpdateDescription(long realmId, DescriptionUpdateRequest request)
    {
        await _realmService.UpdateDescriptionAsync(realmId, request.Name, request.Description);

        return NoContent();
    }

    public override async Task<IActionResult> ActivateSlot(long realmId, int slotId)
    {
        await _realmService.SwitchSlotAsync(realmId, slotId);

        return NoContent();
    }

    public override async Task<IActionResult> Open(long realmId)
    {
        await _realmService.OpenRealmAsync(realmId);

        return NoContent();
    }

    public override async Task<IActionResult> Close(long realmId)
    {
        await _realmService.CloseRealmAsync(realmId);

        return NoContent();
    }

    public override async Task<IActionResult> Reset(long realmId)
    {
        await _realmService.ResetRealmAsync(realmId);

        return NoContent();
    }

    public override async Task<IActionResult> Invite(long realmId, string player)
    {
        await _realmService.InvitePlayerAsync(realmId, player);

        return NoContent();
    }

    public override async Task<IActionResult> Uninvite(long realmId, string player)
    {
        await _realmService.UninvitePlayerAsync(realmId, player);

        return NoContent();
    }

    [HttpPost("Blocklist/{player}")]
    public async Task<IActionResult> BlockPlayer(long realmId, string player)
    {
        await _realmService.BlockPlayerAsync(realmId, player);

        return NoContent();
    }
    
    [HttpDelete("Blocklist/{player}")]
    public async Task<IActionResult> UnblockPlayer(long realmId, string player)
    {
        await _realmService.UnblockPlayerAsync(realmId, player);

        return NoContent();
    }

    [HttpPost("Codes/Invite")]
    public async Task<IActionResult> GenerateInviteLink(long realmId)
    {
        await _realmService.GenerateInviteCodeAsync(realmId);

        return NoContent();
    }

    [HttpPut("Permission/Default")]
    public async Task<IActionResult> ChangeDefaultPermission(long realmId, [FromBody] string permission)
    {
        await _realmService.ChangeDefaultPermissionAsync(realmId, permission);

        return NoContent();
    }
    
    [HttpPut("Permission/Player/{player}")]
    public async Task<IActionResult> ChangePlayerPermission(long realmId, string player, [FromBody] string permission)
    {
        await _realmService.ChangePlayerPermissionAsync(realmId, player, permission);

        return NoContent();
    }
}
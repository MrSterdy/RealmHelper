using Microsoft.AspNetCore.Mvc;

using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Common.Application.Models.Requests;

namespace RealmHelper.Realm.Common.WebApi.Controllers;

[Route("[controller]/{realmId:long}")]
public abstract class RealmController<TRealm, TActivityPlayer, TSlotOptions> : BaseController
    where TRealm : BaseRealm
    where TSlotOptions : SlotOptionsDto
{
    [HttpGet]
    public abstract Task<ActionResult<TRealm>> GetRealm(long realmId, CancellationToken cancellationToken);

    [HttpGet("Activity")]
    public abstract Task<ActionResult<TActivityPlayer[]>> GetOnlinePlayers(long realmId,
        CancellationToken cancellationToken);

    [HttpGet("Backups")]
    public abstract Task<ActionResult<Backup[]>> GetBackups(long realmId, CancellationToken cancellationToken);

    [HttpGet("Slot/{slotId:int}/Backup/Download/{backupId?}")]
    public abstract Task<IActionResult> DownloadBackup(long realmId, int slotId, CancellationToken cancellationToken,
        string? backupId = default);

    [HttpPost("Slot/{slotId:int}/Backup/Upload/{backupId?}")]
    public abstract Task<IActionResult> UploadBackup(long realmId, int slotId, CancellationToken cancellationToken,
        string? backupId = default);

    [HttpPost("Close")]
    public abstract Task<IActionResult> CloseRealm(long realmId, CancellationToken cancellationToken);    
    
    [HttpPost("Open")]
    public abstract Task<IActionResult> OpenRealm(long realmId, CancellationToken cancellationToken);
    
    [HttpPost("Reset")]
    public abstract Task<IActionResult> ResetRealm(long realmId, CancellationToken cancellationToken);

    [HttpPost("Invite/{player}")]
    public abstract Task<IActionResult> InvitePlayer(long realmId, string player, CancellationToken cancellationToken);
    
    [HttpDelete("Invite/{player}")]
    public abstract Task<IActionResult> UninvitePlayer(long realmId, string player, CancellationToken cancellationToken);

    [HttpPut("Description")]
    public abstract Task<IActionResult> UpdateDescription(long realmId, [FromBody] DescriptionUpdateRequest request,
        CancellationToken cancellationToken);

    [HttpPatch("Slot/{slotId:int}")]
    public abstract Task<IActionResult> UpdateSlot(long realmId, int slotId, 
        [FromBody] SlotUpdateRequest<TSlotOptions> request, CancellationToken cancellationToken);

    [HttpPut("Slot/{slotId:int}")]
    public abstract Task<IActionResult> SwitchSlot(long realmId, int slotId, CancellationToken cancellationToken);
}
using System.Text.Json.Serialization;

namespace RealmHelper.Realm.Common.Application.Models.Responses;

public class BackupsResponse
{
    public BackupResponse[] Backups { get; set; } = default!;
}

public class BackupResponse
{
    public string BackupId { get; set; } = default!;
    
    public long Size { get; set; }

    public BackupMetadataResponse Metadata { get; set; } = default!;
}

public class BackupMetadataResponse
{
    public string Name { get; set; } = default!;

    [JsonPropertyName("game_server_version")]
    public string GameServerVersion { get; set; } = default!;
    
    [JsonPropertyName("game_difficulty")]
    public string GameDifficulty { get; set; } = default!;
    
    [JsonPropertyName("game_mode")]
    public string GameMode { get; set; } = default!;
}
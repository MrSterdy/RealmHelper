using System.Text.Json.Serialization;

using RealmHelper.Realm.Common.Application.Models.Responses;

namespace RealmHelper.Realm.Bedrock.Application.Models.Responses;

public class BedrockArchiveDownloadResponse : ArchiveResponse
{
    [JsonPropertyName("downloadUrl")]
    public override string Url { get; set; } = default!;
}

public class BedrockArchiveUploadResponse : ArchiveResponse
{
    [JsonPropertyName("uploadUrl")]
    public override string Url { get; set; } = default!;
}
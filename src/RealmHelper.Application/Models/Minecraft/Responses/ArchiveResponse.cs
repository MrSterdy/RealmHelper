using System.Text.Json.Serialization;

namespace RealmHelper.Application.Models.Minecraft.Responses;

public class ArchiveResponse
{
    public virtual string Url { get; set; } = default!;

    public string? Token { get; set; }
}

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

public class JavaArchiveDownloadResponse : ArchiveResponse
{
    [JsonPropertyName("downloadLink")]
    public override string Url { get; set; } = default!;
}

public class JavaArchiveUploadResponse : ArchiveResponse
{
    [JsonPropertyName("uploadEndpoint")]
    public override string Url { get; set; } = default!;
}
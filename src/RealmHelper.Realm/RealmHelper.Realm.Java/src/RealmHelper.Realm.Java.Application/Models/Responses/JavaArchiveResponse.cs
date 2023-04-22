using System.Text.Json.Serialization;

using RealmHelper.Realm.Common.Application.Models.Responses;

namespace RealmHelper.Realm.Java.Application.Models.Responses;

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
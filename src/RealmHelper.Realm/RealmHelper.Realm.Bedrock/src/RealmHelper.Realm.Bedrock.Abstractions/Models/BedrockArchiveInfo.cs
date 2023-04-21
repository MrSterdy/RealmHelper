namespace RealmHelper.Realm.Bedrock.Abstractions.Models;

public abstract class BedrockArchiveInfo
{
    public string Token { get; set; } = default!;
}

public class BedrockArchiveDownloadInfo : BedrockArchiveInfo
{
    public string DownloadUrl { get; set; } = default!;
}

public class BedrockArchiveUploadInfo : BedrockArchiveInfo
{
    public string UploadUrl { get; set; } = default!;
}
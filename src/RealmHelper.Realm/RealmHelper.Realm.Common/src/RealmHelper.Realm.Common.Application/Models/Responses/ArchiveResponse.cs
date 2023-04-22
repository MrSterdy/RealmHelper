namespace RealmHelper.Realm.Common.Application.Models.Responses;

public class ArchiveResponse
{
    public virtual string Url { get; set; } = default!;

    public string? Token { get; set; }
}
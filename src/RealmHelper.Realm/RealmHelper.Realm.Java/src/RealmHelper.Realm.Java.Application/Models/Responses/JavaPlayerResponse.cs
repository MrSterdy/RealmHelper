using RealmHelper.Realm.Common.Application.Models.Responses;

namespace RealmHelper.Realm.Java.Application.Models.Responses;

public class JavaPlayerResponse : PlayerResponse
{
    public string Name { get; set; } = default!;
}
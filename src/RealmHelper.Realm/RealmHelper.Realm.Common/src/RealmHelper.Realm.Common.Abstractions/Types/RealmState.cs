namespace RealmHelper.Realm.Common.Abstractions.Types;

public static class RealmState
{
    public static readonly string Open = nameof(Open).ToUpper();
    public static readonly string Closed = nameof(Closed).ToUpper();
    public static readonly string Uninitialized = nameof(Uninitialized).ToUpper();
}
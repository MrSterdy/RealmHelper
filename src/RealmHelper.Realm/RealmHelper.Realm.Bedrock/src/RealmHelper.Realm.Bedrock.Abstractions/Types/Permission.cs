namespace RealmHelper.Realm.Bedrock.Abstractions.Types;

public static class Permission
{
    public static readonly string Visitor = nameof(Visitor).ToUpper();
    public static readonly string Member = nameof(Member).ToUpper();
    public static readonly string Operator = nameof(Operator).ToUpper();
}
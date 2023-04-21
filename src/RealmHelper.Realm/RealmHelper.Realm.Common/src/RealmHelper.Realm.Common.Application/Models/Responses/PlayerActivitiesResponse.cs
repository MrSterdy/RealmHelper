namespace RealmHelper.Realm.Common.Application.Models.Responses;

public class PlayerActivitiesResponse<TPlayer>
{
    public PlayerActivityResponse<TPlayer>[] Servers { get; set; } = default!;
}

public class PlayerActivityResponse<TPlayer>
{
    public long RealmId { get; set; }

    public TPlayer[] Players { get; set; } = default!;
}
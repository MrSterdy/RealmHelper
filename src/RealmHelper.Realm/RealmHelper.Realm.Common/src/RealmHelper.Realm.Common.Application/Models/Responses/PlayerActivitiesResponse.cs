namespace RealmHelper.Realm.Common.Application.Models.Responses;

public class PlayerActivitiesResponse<TPlayer, TActivityResponse> where TActivityResponse : PlayerActivityResponse<TPlayer>
{
    public virtual TActivityResponse[] Servers { get; set; } = default!;
}

public class PlayerActivityResponse<TPlayer>
{
    public virtual long RealmId { get; set; }

    public virtual TPlayer[] Players { get; set; } = default!;
}
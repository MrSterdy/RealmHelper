namespace RealmHelper.Realm.Java.Application.Models.Responses;

public class LivePlayerListResponse
{
    public LivePlayersResponse[] Lists { get; set; } = default!;
}

public class LivePlayersResponse
{
    public long ServerId { get; set; }

    public string PlayerList { get; set; } = default!;
}
namespace RealmHelper.Realm.Bedrock.Application.Models.Responses;

public class LivePlayerListResponse
{
    public LivePlayersResponse[] Servers { get; set; } = default!;
}

public class LivePlayersResponse
{
    public long Id { get; set; }

    public BedrockPlayerResponse[] Players { get; set; } = default!;
}
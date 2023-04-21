using RestSharp;

using RealmHelper.Client.Application.Models.Minecraft.Responses;

using RealmHelper.Realm.Java.Abstractions.Models;

namespace RealmHelper.Client.Infrastructure.Utils;

public static class PlayerHelper
{
    private const string Endpoint = "https://playerdb.co/api/player";

    private const string MinecraftPath = "/minecraft/{PLAYER}";
    
    private static readonly RestClient RestClient = new(Endpoint);

    public static async Task<JavaPlayer> FetchPlayer(string player, CancellationToken cancellationToken = default)
    {
        var response =
            await RestClient.GetJsonAsync<PlayerDataResponse>(MinecraftPath.Replace("{PLAYER}", player),
                cancellationToken);

        return new JavaPlayer
        {
            Uuid = response!.Data.Player.RawId,
            Name = response.Data.Player.Username
        };
    }
}
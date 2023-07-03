using RestSharp;

using RealmHelper.Application.Models.Minecraft.Responses;
using RealmHelper.Domain.Models.Minecraft.Java;

namespace RealmHelper.Infrastructure.Utils;

public static class PlayerHelper
{
    private const string Endpoint = "https://playerdb.co/api/player";

    private const string MinecraftPath = "/minecraft/{PLAYER}";
    
    private static readonly RestClient RestClient = new(Endpoint);

    public static async Task<JavaPlayer> FetchPlayer(string player, CancellationToken cancellationToken = default)
    {
        var response =
            await RestClient.GetJsonAsync<JavaPlayerDataResponse>(MinecraftPath.Replace("{PLAYER}", player),
                cancellationToken);

        return new JavaPlayer
        {
            Uuid = response!.Data.Player.RawId,
            Name = response.Data.Player.Username
        };
    }
}
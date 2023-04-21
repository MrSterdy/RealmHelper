using RealmHelper.Client.Application.Models.XboxLive.Responses;

namespace RealmHelper.Client.Application.Repositories;

public interface IProfileRepository
{
    Task<ProfilesResponse> GetProfileAsync(CancellationToken cancellationToken = default);

    Task<ProfilesResponse> GetProfileByXuidAsync(string xuid, CancellationToken cancellationToken = default);

    Task<ProfilesResponse> GetProfileByGamertagAsync(string gamertag, CancellationToken cancellationToken = default);
}
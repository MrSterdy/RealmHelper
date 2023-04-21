using RealmHelper.Client.Domain.Models.XboxLive;

namespace RealmHelper.Client.Application.Services;

public interface IProfileService
{
    Task<Profile> GetProfileAsync(CancellationToken cancellationToken = default);
    
    Task<Profile> GetProfileByXuidAsync(string xuid, CancellationToken cancellationToken = default);

    Task<Profile> GetProfileByGamertagAsync(string gamertag, CancellationToken cancellationToken = default);
}
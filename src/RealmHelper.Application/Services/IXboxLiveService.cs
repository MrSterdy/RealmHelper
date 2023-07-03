using RealmHelper.Domain.Models.XboxLive;

namespace RealmHelper.Application.Services;

public interface IXboxLiveService
{
    Task<Club> GetClubAsync(long clubId, CancellationToken cancellationToken = default);

    Task<ClubActivity[]> GetClubActivitiesAsync(long clubId, int amount, CancellationToken cancellationToken = default);
    
    Task<Profile[]> GetProfilesAsync(string[] xuids, CancellationToken cancellationToken = default);

    Task<Profile> GetProfileAsync(CancellationToken cancellationToken = default);
    
    Task<Profile> GetProfileByXuidAsync(string xuid, CancellationToken cancellationToken = default);

    Task<Profile> GetProfileByGamertagAsync(string gamertag, CancellationToken cancellationToken = default);
}
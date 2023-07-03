using RealmHelper.Application.Models.XboxLive.Responses;

namespace RealmHelper.Application.Api;

public interface IXboxLiveApi
{
    Task<ClubsResponse> GetClubAsync(long clubId, CancellationToken cancellationToken = default);

    Task<ClubActivitiesResponse> GetClubActivitiesAsync(long clubId, int amount,
        CancellationToken cancellationToken = default);
    
    Task<PeopleResponse> GetPeopleAsync(string[] xuids, CancellationToken cancellationToken = default);
    
    Task<ProfilesResponse> GetProfileAsync(CancellationToken cancellationToken = default);

    Task<ProfilesResponse> GetProfileByXuidAsync(string xuid, CancellationToken cancellationToken = default);

    Task<ProfilesResponse> GetProfileByGamertagAsync(string gamertag, CancellationToken cancellationToken = default);
}
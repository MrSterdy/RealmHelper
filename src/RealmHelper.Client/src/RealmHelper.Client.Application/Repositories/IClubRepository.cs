using RealmHelper.Client.Application.Models.XboxLive.Responses;

namespace RealmHelper.Client.Application.Repositories;

public interface IClubRepository
{
    Task<ClubsResponse> GetClubAsync(long clubId, CancellationToken cancellationToken = default);

    Task<ClubActivitiesResponse> GetClubActivitiesAsync(long clubId, int amount,
        CancellationToken cancellationToken = default);
}
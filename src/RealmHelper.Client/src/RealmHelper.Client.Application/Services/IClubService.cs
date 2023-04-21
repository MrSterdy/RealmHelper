using RealmHelper.Client.Domain.Models.XboxLive;

namespace RealmHelper.Client.Application.Services;

public interface IClubService
{
    Task<Club> GetClubAsync(long clubId, CancellationToken cancellationToken = default);

    Task<ClubActivity[]> GetClubActivitiesAsync(long clubId, int amount, CancellationToken cancellationToken = default);
}
using RealmHelper.Client.Domain.Models.XboxLive;

namespace RealmHelper.Client.Application.Services;

public interface IPeopleService
{
    Task<Profile[]> GetProfilesAsync(string[] xuids, CancellationToken cancellationToken = default);
}
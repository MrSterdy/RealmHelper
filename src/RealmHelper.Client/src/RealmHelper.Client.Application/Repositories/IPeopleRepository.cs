using RealmHelper.Client.Application.Models.XboxLive.Responses;

namespace RealmHelper.Client.Application.Repositories;

public interface IPeopleRepository
{
    Task<PeopleResponse> GetPeopleAsync(string[] xuids, CancellationToken cancellationToken = default);
}
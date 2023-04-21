using AutoMapper;

using RealmHelper.Client.Application.Repositories;
using RealmHelper.Client.Application.Services;
using Profile = RealmHelper.Client.Domain.Models.XboxLive.Profile;

namespace RealmHelper.Client.Infrastructure.Services;

public class PeopleService : IPeopleService
{
    private readonly IPeopleRepository _peopleRepository;

    private readonly IMapper _mapper;

    public PeopleService(IPeopleRepository peopleRepository, IMapper mapper) =>
        (_peopleRepository, _mapper) = (peopleRepository, mapper);

    public async Task<Profile[]> GetProfilesAsync(string[] xuids, CancellationToken cancellationToken = default) =>
        _mapper.Map<Profile[]>(await _peopleRepository.GetPeopleAsync(xuids, cancellationToken));
}
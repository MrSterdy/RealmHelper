using AutoMapper;

using RealmHelper.Client.Application.Repositories;
using RealmHelper.Client.Application.Services;
using Profile = RealmHelper.Client.Domain.Models.XboxLive.Profile;

namespace RealmHelper.Client.Infrastructure.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;

    private readonly IMapper _mapper;

    public ProfileService(IProfileRepository profileRepository, IMapper mapper) =>
        (_profileRepository, _mapper) = (profileRepository, mapper);

    public async Task<Profile> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        var response = await _profileRepository.GetProfileAsync(cancellationToken);

        return _mapper.Map<Profile>(response.ProfileUsers![0]);
    }

    public async Task<Profile> GetProfileByXuidAsync(string xuid, CancellationToken cancellationToken = default)
    {
        var response = await _profileRepository.GetProfileByXuidAsync(xuid, cancellationToken);

        return _mapper.Map<Profile>(response.ProfileUsers![0]);
    }

    public async Task<Profile> GetProfileByGamertagAsync(string gamertag, CancellationToken cancellationToken = default)
    {
        var response = await _profileRepository.GetProfileByGamertagAsync(gamertag, cancellationToken);

        return _mapper.Map<Profile>(response.ProfileUsers![0]);
    }
}
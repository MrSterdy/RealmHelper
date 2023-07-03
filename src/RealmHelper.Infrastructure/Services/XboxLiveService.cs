using AutoMapper;

using RealmHelper.Application.Api;
using RealmHelper.Application.Services;
using RealmHelper.Domain.Models.XboxLive;
using XboxProfile = RealmHelper.Domain.Models.XboxLive.Profile;

namespace RealmHelper.Infrastructure.Services;

public class XboxLiveService : IXboxLiveService
{
    private readonly IXboxLiveApi _api;

    private readonly IMapper _mapper;

    public XboxLiveService(IXboxLiveApi api, IMapper mapper) =>
        (_api, _mapper) = (api, mapper);
    
    public async Task<Club> GetClubAsync(long clubId, CancellationToken cancellationToken = default)
    {
        var response = await _api.GetClubAsync(clubId, cancellationToken);

        return _mapper.Map<Club>(response.Clubs[0]);
    }

    public async Task<ClubActivity[]> GetClubActivitiesAsync(long clubId, int amount,
        CancellationToken cancellationToken = default) =>
        _mapper.Map<ClubActivity[]>(await _api.GetClubActivitiesAsync(clubId, amount, cancellationToken));
    
    public async Task<XboxProfile[]> GetProfilesAsync(string[] xuids, CancellationToken cancellationToken = default) =>
        _mapper.Map<XboxProfile[]>(await _api.GetPeopleAsync(xuids, cancellationToken));
    
    public async Task<XboxProfile> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        var response = await _api.GetProfileAsync(cancellationToken);

        return _mapper.Map<XboxProfile>(response.ProfileUsers![0]);
    }

    public async Task<XboxProfile> GetProfileByXuidAsync(string xuid, CancellationToken cancellationToken = default)
    {
        var response = await _api.GetProfileByXuidAsync(xuid, cancellationToken);

        return _mapper.Map<XboxProfile>(response.ProfileUsers![0]);
    }

    public async Task<XboxProfile> GetProfileByGamertagAsync(string gamertag, CancellationToken cancellationToken = default)
    {
        var response = await _api.GetProfileByGamertagAsync(gamertag, cancellationToken);

        return _mapper.Map<XboxProfile>(response.ProfileUsers![0]);
    }
}
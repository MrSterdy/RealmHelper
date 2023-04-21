using AutoMapper;

using RealmHelper.Client.Application.Repositories;
using RealmHelper.Client.Application.Services;
using RealmHelper.Client.Domain.Models.XboxLive;

namespace RealmHelper.Client.Infrastructure.Services;

public class ClubService : IClubService
{
    private readonly IClubRepository _clubRepository;

    private readonly IMapper _mapper;

    public ClubService(IClubRepository clubRepository, IMapper mapper) =>
        (_clubRepository, _mapper) = (clubRepository, mapper);

    public async Task<Club> GetClubAsync(long clubId, CancellationToken cancellationToken = default)
    {
        var response = await _clubRepository.GetClubAsync(clubId, cancellationToken);

        return _mapper.Map<Club>(response.Clubs[0]);
    }

    public async Task<ClubActivity[]> GetClubActivitiesAsync(long clubId, int amount,
        CancellationToken cancellationToken = default) =>
        _mapper.Map<ClubActivity[]>(await _clubRepository.GetClubActivitiesAsync(clubId, amount, cancellationToken));
}
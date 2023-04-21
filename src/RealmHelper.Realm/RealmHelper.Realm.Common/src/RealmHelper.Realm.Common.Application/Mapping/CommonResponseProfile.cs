using AutoMapper;

using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Common.Application.Models.Responses;

namespace RealmHelper.Realm.Common.Application.Mapping;

public class CommonResponseProfile : Profile
{
    public CommonResponseProfile()
    {
        CreateMap<PlayerResponse, Player>();
        
        CreateMap<BackupMetadataResponse, BackupMetadata>()
            .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.GameServerVersion))
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => int.Parse(src.GameDifficulty)))
            .ForMember(dest => dest.GameMode, opt => opt.MapFrom(src => int.Parse(src.GameMode)));
        CreateMap<BackupResponse, Backup>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BackupId));
        CreateMap<BackupsResponse, Backup[]>()
            .ConvertUsing((src, _, ctx) => ctx.Mapper.Map<Backup[]>(src.Backups));
    }
}
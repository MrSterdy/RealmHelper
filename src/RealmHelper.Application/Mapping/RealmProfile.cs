using AutoMapper;

using OneOf;

using RealmHelper.Application.Models.Minecraft;
using RealmHelper.Application.Models.Minecraft.Responses;
using RealmHelper.Domain.Models.Minecraft;
using RealmHelper.Domain.Models.Minecraft.Bedrock;
using RealmHelper.Domain.Models.Minecraft.Java;

namespace RealmHelper.Application.Mapping;

public class RealmProfile : Profile
{
    public RealmProfile()
    {
        CreateMap<BackupMetadataResponse, BackupMetadata>()
            .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.GameServerVersion))
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => int.Parse(src.GameDifficulty)))
            .ForMember(dest => dest.GameMode, opt => opt.MapFrom(src => int.Parse(src.GameMode)));
        CreateMap<BackupResponse, Backup>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BackupId));
        CreateMap<BackupsResponse, Backup[]>()
            .ConvertUsing((src, _, ctx) => ctx.Mapper.Map<Backup[]>(src.Backups));
        
        CreateMap<BedrockSlotOptionsDto.WorldSetting, OneOf<bool, int, object>>()
            .ConvertUsing(src => OneOf<bool, int, object>.FromT2(src.Value));
        CreateMap<BedrockSlotOptionsDto.Packs, BedrockSlotOptions.Packs>();
        CreateMap<BedrockSlotOptionsDto, BedrockSlotOptions>();
        CreateMap<BedrockSlotResponse, Slot<BedrockSlotOptions>>();
        CreateMap<BedrockPlayerResponse, BedrockPlayer>();
        CreateMap<BedrockRealmResponse, BedrockRealm>();
        CreateMap<RealmsResponse<BedrockRealmResponse>, BedrockRealm[]>()
            .ConvertUsing((src, _, ctx) => ctx.Mapper.Map<BedrockRealm[]>(src.Servers));
        
        CreateMap<SlotOptionsDto, SlotOptions>();
        CreateMap<JavaSlotResponse, Slot<SlotOptions>>();
        CreateMap<JavaPlayerResponse, JavaPlayer>();
        CreateMap<JavaRealmResponse, JavaRealm>();
        CreateMap<RealmsResponse<JavaRealmResponse>, JavaRealm[]>()
            .ConvertUsing((src, _, ctx) => ctx.Mapper.Map<JavaRealm[]>(src.Servers));
    }
}
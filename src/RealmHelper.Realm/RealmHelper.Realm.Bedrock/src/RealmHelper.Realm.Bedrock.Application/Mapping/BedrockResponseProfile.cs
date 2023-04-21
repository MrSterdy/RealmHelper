using AutoMapper;

using OneOf;

using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Bedrock.Application.Models.Responses;
using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Common.Application.Models.Responses;

namespace RealmHelper.Realm.Bedrock.Application.Mapping;

public class BedrockResponseProfile : Profile
{
    public BedrockResponseProfile()
    {
        CreateMap<WorldSetting, OneOf<bool, int>>()
            .ConvertUsing(src => src.Value);
        CreateMap<BedrockSlotOptionsDto.Packs, BedrockSlotOptions.Packs>();
        CreateMap<BedrockSlotOptionsDto, BedrockSlotOptions>();
        CreateMap<BedrockSlotResponse, Slot<BedrockSlotOptions>>();
        CreateMap<BedrockPlayerResponse, BedrockPlayer>()
            .IncludeBase<PlayerResponse, Player>();
        CreateMap<BedrockRealmResponse, BedrockRealm>();
        CreateMap<RealmsResponse<BedrockRealmResponse>, BedrockRealm[]>()
            .ConvertUsing((src, _, ctx) => ctx.Mapper.Map<BedrockRealm[]>(src.Servers));
    }
}
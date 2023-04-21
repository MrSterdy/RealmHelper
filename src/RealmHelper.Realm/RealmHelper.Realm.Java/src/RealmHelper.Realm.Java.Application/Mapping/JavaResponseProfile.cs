using AutoMapper;

using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Common.Application.Models.Responses;
using RealmHelper.Realm.Java.Abstractions.Models;
using RealmHelper.Realm.Java.Application.Models.Responses;

namespace RealmHelper.Realm.Java.Application.Mapping;

public class JavaResponseProfile : Profile
{
    public JavaResponseProfile()
    {
        CreateMap<SlotOptionsDto, SlotOptions>();
        CreateMap<JavaSlotResponse, Slot<SlotOptions>>();
        CreateMap<JavaPlayerResponse, JavaPlayer>()
            .IncludeBase<PlayerResponse, Player>();
        CreateMap<JavaRealmResponse, JavaRealm>();
        CreateMap<RealmsResponse<JavaRealmResponse>, JavaRealm[]>()
            .ConvertUsing((src, _, ctx) => ctx.Mapper.Map<JavaRealm[]>(src.Servers));
    }
}
using AutoMapper;

using RealmHelper.Client.Application.Models.Minecraft.Responses;
using RealmHelper.Client.Domain.Models.Minecraft;

namespace RealmHelper.Client.Application.Mapping;

public class MinecraftResponseMapper : Profile
{
    public MinecraftResponseMapper()
    {
        CreateMap<PlayerDataResponse, PlayerData>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Data.Player.Username))
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Data.Player.RawId));
    }
}
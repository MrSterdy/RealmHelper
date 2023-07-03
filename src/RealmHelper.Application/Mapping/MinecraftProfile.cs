using AutoMapper;

using RealmHelper.Application.Models.Minecraft.Responses;
using RealmHelper.Domain.Models.Minecraft.Java;

namespace RealmHelper.Application.Mapping;

public class MinecraftProfile : Profile
{
    public MinecraftProfile()
    {
        CreateMap<JavaPlayerDataResponse, JavaPlayerData>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Data.Player.Username))
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Data.Player.RawId));
    }
}
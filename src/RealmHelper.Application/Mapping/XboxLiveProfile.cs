using Profile = AutoMapper.Profile;

using RealmHelper.Application.Models.XboxLive.Responses;
using RealmHelper.Domain.Models.XboxLive;
using XboxProfile = RealmHelper.Domain.Models.XboxLive.Profile;

namespace RealmHelper.Application.Mapping;

public class XboxLiveProfile : Profile
{
    public XboxLiveProfile()
    {
        CreateMap<ClubProfileResponse, ClubProfile>();
        CreateMap<ClubProfileResponse.ProfileEntry, string>()
            .ConvertUsing(src => src.Value);
        CreateMap<ClubMemberResponse, ClubMember>();
        CreateMap<ClubResponse, Club>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom((src, _) => long.Parse(src.Id)))
            .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDateUtc))
            .ForMember(dest => dest.OwnerXuid, opt => opt.MapFrom(src => src.OwnerXuid))
            .ForMember(dest => dest.Members, opt => opt.MapFrom((src, _, _, ctx) =>
                ctx.Mapper.Map<ClubMember[]>(src.ClubPresence)));
        CreateMap<ClubsResponse, Club[]>()
            .ConvertUsing((src, _, ctx) => ctx.Mapper.Map<Club[]>(src.Clubs));
        
        CreateMap<ClubActivityAuthorResponse, XboxProfile>()
            .ForMember(dest => dest.Gamertag, opt => opt.MapFrom((src, _) => src.ModernGamertag + src.ModernGamertagSuffix))
            .ForMember(dest => dest.DisplayImageUrl, opt => opt.MapFrom(src => src.ImageUrl));
        CreateMap<ClubActivityResponse, ClubActivity>()
            .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.UgcCaption))
            .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.NumLikes))
            .ForMember(dest => dest.Author, opt => opt.MapFrom((src, _, _, ctx) => 
                ctx.Mapper.Map<XboxProfile>(src.AuthorInfo)));
        CreateMap<ClubActivitiesResponse, ClubActivity[]>()
            .ConvertUsing((src, _, ctx) => ctx.Mapper.Map<ClubActivity[]>(src.ActivityItems));
        
        CreateMap<ProfileResponse, XboxProfile>()
            .ForMember(dest => dest.Xuid, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.DisplayImageUrl, opt => opt.MapFrom(src => src.GameDisplayPicRaw));
        CreateMap<ProfilesResponse, XboxProfile[]>()
            .ConvertUsing((src, _, ctx) => ctx.Mapper.Map<XboxProfile[]>(src.ProfileUsers));
        
        CreateMap<PersonResponse, XboxProfile>()
            .ForMember(dest => dest.DisplayImageUrl, opt => opt.MapFrom(src => src.DisplayPicRaw));
        CreateMap<PeopleResponse, XboxProfile[]>()
            .ConvertUsing((src, _, ctx) => ctx.Mapper.Map<XboxProfile[]>(src.People));
    }
}
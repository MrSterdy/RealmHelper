using RealmHelper.Client.Domain.Models.XboxLive;

namespace RealmHelper.Client.Application.Models.XboxLive.Responses;

public class ClubsResponse
{
    public ClubResponse[] Clubs { get; set; } = default!;
}

public class ClubResponse
{
    public string Id { get; set; } = default!;
    
    public DateTimeOffset CreationDateUtc { get; set; }

    public string OwnerXuid { get; set; } = default!;

    public ClubProfileResponse Profile { get; set; } = default!;
    
    public int MembersCount { get; set; }

    public ClubMember[] ClubPresence { get; set; } = default!;
}

public class ClubProfileResponse
{
    public struct ProfileEntry
    {
        public string Value { get; set; }
    }
    
    public ProfileEntry Name { get; set; }
    public ProfileEntry Description { get; set; }
        
    public ProfileEntry DisplayImageUrl { get; set; }
    public ProfileEntry BackgroundImageUrl { get; set; }
}
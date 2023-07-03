namespace RealmHelper.Application.Models.XboxLive.Responses;

public class ClubsResponse
{
    public ClubResponse[] Clubs { get; set; } = default!;
}

public class ClubResponse
{
    public string Id { get; set; } = default!;

    public string CreationDateUtc { get; set; } = default!;

    public string OwnerXuid { get; set; } = default!;

    public ClubProfileResponse Profile { get; set; } = default!;
    
    public int MembersCount { get; set; }

    public ClubMemberResponse[] ClubPresence { get; set; } = default!;
}

public class ClubMemberResponse
{
    public string Xuid { get; set; } = default!;

    public string LastSeenTimestamp { get; set; } = default!;

    public string LastSeenState { get; set; } = default!;
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
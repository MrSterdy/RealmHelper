namespace RealmHelper.Domain.Models.XboxLive;

public class Club
{
    public long Id { get; set; }
    
    public DateTimeOffset CreationDate { get; set; }

    public string OwnerXuid { get; set; } = default!;

    public ClubProfile Profile { get; set; } = default!;
    
    public int MembersCount { get; set; }

    public ClubMember[] Members { get; set; } = default!;
}

public class ClubProfile
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    
    public string DisplayImageUrl { get; set; } = default!;
    public string BackgroundImageUrl { get; set; } = default!;
}

public class ClubMember
{
    public string Xuid { get; set; } = default!;

    public DateTimeOffset LastSeenTimestamp { get; set; }

    public string LastSeenState { get; set; } = default!;
}

public class ClubActivity
{
    public Profile Author { get; set; } = default!;
    
    public DateTimeOffset CreationDate { get; set; }

    public string ShortDescription { get; set; } = default!;
    
    public string Text { get; set; } = default!;
    public string? ScreenshotUri { get; set; }
        
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
}
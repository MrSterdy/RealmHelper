namespace RealmHelper.Client.Application.Models.XboxLive.Responses;

public class ClubActivitiesResponse
{
    public ClubActivityResponse[] ActivityItems { get; set; } = default!;
}

public class ClubActivityResponse
{
    public DateTimeOffset Date { get; set; }

    public string ShortDescription { get; set; } = default!;
    
    public string UgcCaption { get; set; } = default!;
        
    public string? ScreenshotUri { get; set; }
        
    public int ViewCount { get; set; }
    public int NumLikes { get; set; }

    public ClubActivityAuthorResponse AuthorInfo { get; set; } = default!;
}

public class ClubActivityAuthorResponse
{
    public string ModernGamertag { get; set; } = default!;
    public string ModernGamertagSuffix { get; set; } = default!;

    public string ImageUrl { get; set; } = default!;
}
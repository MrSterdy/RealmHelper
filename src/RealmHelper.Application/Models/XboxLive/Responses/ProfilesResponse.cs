namespace RealmHelper.Application.Models.XboxLive.Responses;

public class ProfilesResponse
{
    public ProfileResponse[]? ProfileUsers { get; set; } = default!;
}

public static class ProfileProperty
{
    public const string Gamertag = nameof(Gamertag);

    public const string GameDisplayPicRaw = nameof(GameDisplayPicRaw);
}

public class ProfileResponse
{
    public struct Setting
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
    
    public string Id { get; set; } = default!;

    public Setting[] Settings { get; set; } = default!;

    public string this[string key] =>
        Settings.FirstOrDefault(s => s.Id == key).Value;

    public string Gamertag => this[ProfileProperty.Gamertag];
    
    public string GameDisplayPicRaw => this[ProfileProperty.GameDisplayPicRaw];
}
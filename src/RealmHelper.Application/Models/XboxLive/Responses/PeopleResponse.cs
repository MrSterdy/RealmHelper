namespace RealmHelper.Application.Models.XboxLive.Responses;

public class PeopleResponse
{
    public PersonResponse[] People { get; set; } = default!;
}

public class PersonResponse
{
    public string Xuid { get; set; } = default!;

    public string Gamertag { get; set; } = default!;

    public string DisplayPicRaw { get; set; } = default!;
}
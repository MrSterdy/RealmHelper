namespace RealmHelper.Client.Infrastructure;

public class WebApiOptions
{
    public const string Section = "WebApi";
    
    public Uri Endpoint { get; set; } = default!;
}
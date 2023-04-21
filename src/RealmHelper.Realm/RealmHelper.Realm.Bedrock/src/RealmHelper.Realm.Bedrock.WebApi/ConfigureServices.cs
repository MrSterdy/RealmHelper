using RealmHelper.Realm.Common.WebApi;

namespace RealmHelper.Realm.Bedrock.WebApi;

public static class ConfigureServices
{
    public static void AddBedrockWebApiServices(this IServiceCollection services) =>
        services.AddCommonWebApiServices();
}
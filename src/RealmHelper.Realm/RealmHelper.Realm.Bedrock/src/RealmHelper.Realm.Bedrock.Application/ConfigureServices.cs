using Microsoft.Extensions.DependencyInjection;

using RealmHelper.Realm.Bedrock.Application.Mapping;
using RealmHelper.Realm.Common.Application;

namespace RealmHelper.Realm.Bedrock.Application;

public static class ConfigureServices
{
    public static void AddBedrockApplicationServices(this IServiceCollection services)
    {
        services.AddCommonApplicationServices();

        services.AddAutoMapper(typeof(BedrockResponseProfile));
    }
}
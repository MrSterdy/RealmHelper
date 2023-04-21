using Microsoft.Extensions.DependencyInjection;

using RealmHelper.Client.Application.Mapping;

namespace RealmHelper.Client.Application;

public static class ConfigureServices
{
    public static void AddApplicationServices(this IServiceCollection services) =>
        services.AddAutoMapper(typeof(XboxLiveResponseMapper), typeof(MinecraftResponseMapper));
}
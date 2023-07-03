using RealmHelper.Application.Mapping;

using Microsoft.Extensions.DependencyInjection;

namespace RealmHelper.Application;

public static class ConfigureServices
{
    public static void AddApplicationServices(this IServiceCollection services) =>
        services.AddAutoMapper(typeof(MinecraftProfile), typeof(XboxLiveProfile), typeof(RealmProfile));
}
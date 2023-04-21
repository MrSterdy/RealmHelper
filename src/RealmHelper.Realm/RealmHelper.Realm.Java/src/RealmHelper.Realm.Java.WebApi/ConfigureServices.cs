using RealmHelper.Realm.Common.WebApi;

namespace RealmHelper.Realm.Java.WebApi;

public static class ConfigureServices
{
    public static void AddJavaWebApiServices(this IServiceCollection services) =>
        services.AddCommonWebApiServices();
}
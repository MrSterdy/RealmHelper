﻿namespace RealmHelper.Web;

public static class ConfigureServices
{
    public static void AddWebUiServices(this IServiceCollection services) =>
        services.AddRazorPages();
}
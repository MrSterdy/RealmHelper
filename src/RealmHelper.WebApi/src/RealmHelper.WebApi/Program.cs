using Ocelot.Configuration.File;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddOcelot(builder.Environment);

builder.Services.AddOcelot();

builder.Services.PostConfigure<FileConfiguration>(cfg =>
{
    var services = builder.Configuration.GetSection("Services").Get<Dictionary<string, Uri>>()!;

    foreach (var route in cfg.Routes)
        foreach (var hostAndPort in route.DownstreamHostAndPorts)
        {
            var host = hostAndPort.Host;
            if (!host.StartsWith("{") || !host.EndsWith("}"))
                continue;
            
            var placeHolder = host.TrimStart('{').TrimEnd('}');
            if (!services.TryGetValue(placeHolder, out var uri))
                continue;
            
            route.DownstreamScheme = uri.Scheme;
            hostAndPort.Host = uri.Host;
            hostAndPort.Port = uri.Port;
        }
});

var app = builder.Build();

app.UseOcelot().Wait();

app.Run();
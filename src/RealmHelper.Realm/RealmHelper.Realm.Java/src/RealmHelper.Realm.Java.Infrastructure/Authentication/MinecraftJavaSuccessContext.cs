using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace RealmHelper.Realm.Java.Infrastructure.Authentication;

public class MinecraftJavaSuccessContext : ResultContext<MinecraftJavaOptions>
{
    public MinecraftJavaSuccessContext(
        HttpContext context, 
        AuthenticationScheme scheme, 
        MinecraftJavaOptions options
    ) : base(context, scheme, options)
    {
    }
}
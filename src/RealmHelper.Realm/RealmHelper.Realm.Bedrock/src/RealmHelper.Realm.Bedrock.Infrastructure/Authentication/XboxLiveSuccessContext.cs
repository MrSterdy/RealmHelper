using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace RealmHelper.Realm.Bedrock.Infrastructure.Authentication;

public class XboxLiveSuccessContext : ResultContext<XboxLiveOptions>
{
    public XboxLiveSuccessContext(HttpContext context, AuthenticationScheme scheme, XboxLiveOptions options) : base(context, scheme, options)
    {
    }
}
using System.Security.Claims;

using RealmHelper.Client.Infrastructure.Authentication;
using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Java.Abstractions.Models;

namespace RealmHelper.Client.Infrastructure.Utils;

public static class RealmHelper
{
    public static bool IsOwner(this BaseRealm realm, ClaimsPrincipal user) =>
        realm.OwnerUuid == (realm is JavaRealm
            ? user.FindFirstValue(AuthClaims.JavaUuid)
            : user.FindFirstValue(AuthClaims.BedrockUuid));
}
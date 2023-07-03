using System.Security.Claims;

using RealmHelper.Application.Authentication;
using RealmHelper.Domain.Models.Minecraft;
using RealmHelper.Domain.Models.Minecraft.Java;

namespace RealmHelper.Application.Utils;

public static class RealmHelper
{
    public static bool IsOwner(this BaseRealm realm, ClaimsPrincipal user) =>
        realm.OwnerUuid == (realm is JavaRealm
            ? user.FindFirst(AuthClaims.JavaUuid)
            : user.FindFirst(AuthClaims.BedrockUuid))?.Value;
}
using System.Security.Claims;

namespace Identification.Constants;

public static class ClaimConstants
{
    public const string ExternalUserId = "http://schemas.microsoft.com/identity/claims/objectidentifier";

    public const string InternalUserId = "user_id";

    public const string Role = ClaimTypes.Role;

    public const string Name = "name";

    public const string Email = "preferred_username";
}

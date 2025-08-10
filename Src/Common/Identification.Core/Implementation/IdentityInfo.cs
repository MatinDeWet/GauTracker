using System.Security.Claims;
using Identification.Base;

namespace Identification.Core.Implementation;
public class IdentityInfo : IIdentityInfo
{
    private readonly IInfoSetter _infoSetter;

    public IdentityInfo(IInfoSetter infoSetter)
    {
        _infoSetter = infoSetter;
    }

    public Guid GetIdentityId()
    {
        string uid = GetValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(uid) || !Guid.TryParse(uid, out Guid result))
        {
            throw new InvalidOperationException("The identity ID is not set or is not a valid GUID.");
        }

        return result;
    }

    public bool HasRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            throw new ArgumentException("Role cannot be null, empty, or whitespace.", nameof(role));
        }

        IEnumerable<string> roles = _infoSetter
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .Where(x => !string.IsNullOrWhiteSpace(x));

        return roles.Any(roleString =>
            roleString.Equals(role, StringComparison.OrdinalIgnoreCase));
    }

    public string GetValue(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null, empty, or whitespace.", nameof(name));
        }

        Claim? claim = _infoSetter.FirstOrDefault(x => x.Type == name);
        return claim?.Value ?? string.Empty;
    }

    public bool HasValue(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null, empty, or whitespace.", nameof(name));
        }

        return _infoSetter.Any(x => x.Type == name);
    }
}

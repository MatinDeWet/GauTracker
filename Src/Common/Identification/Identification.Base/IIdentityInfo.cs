namespace Identification.Base;

public interface IIdentityInfo
{
    Guid GetIdentityId();

    bool HasRole(string role);

    bool HasValue(string name);

    string GetValue(string name);
}

using System.Security.Claims;

namespace Identification.Base;
public interface IInfoSetter : IList<Claim>
{
    void SetUser(IEnumerable<Claim> claims);
}

using System.Security.Claims;
using Identification.Base;

namespace Identification.Core.Implementation;
public class InfoSetter : List<Claim>, IInfoSetter
{
    public void SetUser(IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims);

        Clear();
        AddRange(claims);
    }

    public virtual new void Clear()
    {
        base.Clear();
    }

    public virtual new void AddRange(IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims);

        base.AddRange(claims);
    }
}

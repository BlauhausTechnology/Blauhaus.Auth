using System.Collections.Generic;

namespace Blauhaus.Auth.Abstractions.User;

public interface IHasClaims
{
    IReadOnlyDictionary<string, string> Claims { get; }
}
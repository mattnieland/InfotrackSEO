using Microsoft.AspNetCore.Authorization;

namespace YAMDB.Api.Authentication;

/// <summary>
///     The Scope authorization requirement
/// </summary>
public class HasScopeRequirement : IAuthorizationRequirement
{
    /// <summary>
    ///     Public constructor for scope requirement
    /// </summary>
    /// <param name="scope">The scope to check</param>
    /// <param name="issuer">The issuer of the authorization</param>
    /// <exception cref="ArgumentNullException"></exception>
    public HasScopeRequirement(string scope, string issuer)
    {
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
    }

    /// <summary>
    ///     The issuer
    /// </summary>
    public string Issuer { get; }

    /// <summary>
    ///     The scope
    /// </summary>
    public string Scope { get; }
}
using Microsoft.AspNetCore.Authentication;

namespace TeamUp.Security;

public sealed class JWTCookieSchemeOptions : AuthenticationSchemeOptions
{
	public static readonly string Scheme = "JWT";
}

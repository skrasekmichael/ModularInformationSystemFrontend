using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

namespace TeamUp.Security;

public sealed class JWTCookieSchemeHandler : AuthenticationHandler<JWTCookieSchemeOptions>
{
	public JWTCookieSchemeHandler(IOptionsMonitor<JWTCookieSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder) { }

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		if (Request.Cookies.TryGetValue("JWT", out var jwtCookie))
		{
			var handler = new JwtSecurityTokenHandler();
			var jsonToken = handler.ReadJwtToken(jwtCookie);
			var identity = new ClaimsIdentity(jsonToken.Claims, "JWT");
			var user = new ClaimsPrincipal(identity);

			var result = AuthenticateResult.Success(new AuthenticationTicket(user, JWTCookieSchemeOptions.Scheme));
			return Task.FromResult(result);
		}

		return Task.FromResult(AuthenticateResult.Fail("Missing JWT cookie"));
	}

	protected override Task HandleChallengeAsync(AuthenticationProperties properties)
	{
		if (Request.Cookies.TryGetValue("JWT", out var jwtCookie))
		{
			var handler = new JwtSecurityTokenHandler();
			var jsonToken = handler.ReadJwtToken(jwtCookie);
			var identity = new ClaimsIdentity(jsonToken.Claims, "JWT");
			var user = new ClaimsPrincipal(identity);

			var result = AuthenticateResult.Success(new AuthenticationTicket(user, JWTCookieSchemeOptions.Scheme));
			return Task.FromResult(result);
		}

		Request.HttpContext.Response.Redirect($"/login?returnUrl={Request.GetEncodedUrl()}");
		return Task.FromResult(AuthenticateResult.Fail("Missing JWT cookie"));
	}
}

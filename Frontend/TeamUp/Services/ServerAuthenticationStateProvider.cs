using System.Diagnostics;
using System.Security.Claims;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;

using TeamUp.Client.Services;

namespace TeamUp.Services;

internal sealed class ServerAuthenticationStateProvider : Microsoft.AspNetCore.Components.Server.ServerAuthenticationStateProvider, IDisposable
{
	private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

	private readonly PersistentComponentState _state;
	private readonly PersistingComponentStateSubscription _subscription;
	private readonly ILogger<ServerAuthenticationStateProvider> _logger;
	private Task<AuthenticationState>? _authenticationStateTask;
	private readonly IHttpContextAccessor _contextAccessor;

	public ServerAuthenticationStateProvider(PersistentComponentState persistentComponentState, ILogger<ServerAuthenticationStateProvider> logger, IHttpContextAccessor contextAccessor)
	{
		_state = persistentComponentState;

		AuthenticationStateChanged += OnAuthenticationStateChanged;
		_subscription = _state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
		_logger = logger;
		_contextAccessor = contextAccessor;
	}

	private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
	{
		_authenticationStateTask = task;
	}

	private async Task OnPersistingAsync()
	{
		if (_authenticationStateTask is null)
		{
			throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
		}

		var authenticationState = await _authenticationStateTask;
		var principal = authenticationState.User;

		if (principal.Identity?.IsAuthenticated == true)
		{
			var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
			var email = principal.FindFirstValue(ClaimTypes.Email);
			var username = principal.FindFirstValue(ClaimTypes.Name);

			if (userId is not null && email is not null && username is not null)
			{
				_logger.LogInformation("Logged in as {name}", username);
				_state.PersistAsJson(nameof(UserInfo), new UserInfo
				{
					UserId = userId,
					Email = email,
					Username = username
				});
			}
		}
	}

	public void Dispose()
	{
		_subscription.Dispose();
		AuthenticationStateChanged -= OnAuthenticationStateChanged;
	}

	public void Logout()
	{
		_logger.LogInformation("Logged out");
		NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
	}
}

using BitzArt.Blazor.Cookies;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components;

using TeamUp.ApiLayer;
using TeamUp.Components;
using TeamUp.Security;
using TeamUp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
	.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

builder.AddBlazorCookies();
builder.Services.AddFluentUIComponents();

builder.Services.AddScoped<IMessenger, WeakReferenceMessenger>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddApiClient(builder.Configuration["BackendUrl"]!);

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

builder.Services
	.AddAuthentication(JWTCookieSchemeOptions.Scheme)
	.AddScheme<JWTCookieSchemeOptions, JWTCookieSchemeHandler>(JWTCookieSchemeOptions.Scheme, options => { });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(TeamUp.Client._Imports).Assembly);

app.Run();

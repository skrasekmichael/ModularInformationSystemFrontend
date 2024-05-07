using BitzArt.Blazor.Cookies;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

using TeamUp.Client.Services;
using TeamUp.DAL;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.AddBlazorCookies();
builder.Services.AddFluentUIComponents();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, ClientAuthenticationStateProvider>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddClientDAL(builder.Configuration["BackendUrl"]!);

var app = builder.Build();

await app.RunAsync();

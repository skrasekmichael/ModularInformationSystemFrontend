using BitzArt.Blazor.Cookies;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

using TeamUp.ApiLayer;
using TeamUp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.AddBlazorCookies();
builder.Services.AddFluentUIComponents();

builder.Services.AddScoped<IMessenger, WeakReferenceMessenger>();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, ClientAuthenticationStateProvider>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddApiClient(builder.Configuration["BackendUrl"]!);

var app = builder.Build();

await app.RunAsync();

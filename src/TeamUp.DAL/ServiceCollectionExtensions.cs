﻿using Blazored.LocalStorage;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.DependencyInjection;

using TeamUp.DAL.Api;
using TeamUp.DAL.Cache;
using TeamUp.DAL.Services;

namespace TeamUp.DAL;

public static class ServiceCollectionExtensions
{
	private static void AddDAL(this IServiceCollection services, string api)
	{
		services.AddBlazoredLocalStorage();

		services
			.AddHttpClient<ApiClient>("ApiHttpClient")
			.ConfigureHttpClient(client =>
			{
				client.BaseAddress = new Uri(api);
			});

		services.AddScoped<IMessenger, WeakReferenceMessenger>();
		services.AddScoped<CacheFacade>();

		services
			.AddScoped<TeamService>()
			.AddScoped<InvitationsService>()
			.AddScoped<LoginService>();
	}

	public static void AddClientDAL(this IServiceCollection services, string api)
	{
		services.AddDAL(api);
		services.AddScoped<ICacheStorage, ClientCacheStorage>();
	}

	public static void AddServerDAL(this IServiceCollection services, string api)
	{
		services.AddDAL(api);
		services.AddScoped<ICacheStorage, ServerCacheStorage>();
	}
}
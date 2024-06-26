﻿using RailwayResult;

using TeamUp.Contracts.Users;

namespace TeamUp.DAL.Api;

public sealed partial class ApiClient
{
	private const string HTTP_HEADER_PASSWORD = "HTTP_HEADER_PASSWORD";

	public Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken ct) =>
		SendAsync<LoginRequest, string>(HttpMethod.Post, "/api/v1/users/login", request, ct);

	public Task<Result<UserId>> RegisterAsync(RegisterUserRequest request, CancellationToken ct) =>
		SendAsync<RegisterUserRequest, UserId>(HttpMethod.Post, "/api/v1/users/register", request, ct);

	public Task<Result> ActivateAsync(UserId userId, CancellationToken ct) =>
		SendAsync(HttpMethod.Post, $"/api/v1/users/{userId.Value}/activate", ct);

	public Task<Result> CompleteRegistrationAsync(UserId userId, string password, CancellationToken ct)
	{
		return SendAsync(HttpMethod.Post, $"/api/v1/users/{userId.Value}/generated/complete", request =>
		{
			request.Headers.Add(HTTP_HEADER_PASSWORD, password);
			return true;
		}, ct);
	}

	public Task<Result> DeleteAccountAsync(string password, CancellationToken ct)
	{
		return SendAsync(HttpMethod.Delete, $"/api/v1/users", request =>
		{
			request.Headers.Add(HTTP_HEADER_PASSWORD, password);
			return false;
		}, ct);
	}
}

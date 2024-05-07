using RailwayResult;

using TeamUp.Contracts.Users;

namespace TeamUp.DAL.Api;

public sealed partial class ApiClient
{
	private const string HTTP_HEADER_CONFIRM_PASSWORD = "HTTP_HEADER_CONFIRM_PASSWORD";

	public Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken ct) =>
		SendAsync<LoginRequest, string>(HttpMethod.Post, "/api/v1/users/login", request, ct);

	public Task<Result<UserId>> RegisterAsync(RegisterUserRequest request, CancellationToken ct) =>
		SendAsync<RegisterUserRequest, UserId>(HttpMethod.Post, "/api/v1/users/register", request, ct);
}

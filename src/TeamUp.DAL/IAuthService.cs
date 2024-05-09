using TeamUp.Contracts.Users;

namespace TeamUp.DAL;

public interface IAuthService
{
	public Task<string?> GetTokenAsync(CancellationToken ct = default);

	public Task<UserId> GetUserIdAsync();

	public Task LogoutAsync(string url = "/login", CancellationToken ct = default);
}

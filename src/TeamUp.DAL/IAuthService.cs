namespace TeamUp.DAL;

public interface IAuthService
{
	public Task<string?> GetTokenAsync(CancellationToken ct = default);

	public Task LogoutAsync(string url = "/login", CancellationToken ct = default);
}

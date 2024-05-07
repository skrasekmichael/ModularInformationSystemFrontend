using Microsoft.AspNetCore.Components;

namespace TeamUp.Client.Components;

public abstract class CancellableComponent : ComponentBase, IDisposable
{
	protected CancellationTokenSource CTS { get; } = new();

	public virtual void Dispose()
	{
		CTS.Cancel();
		CTS.Dispose();
	}
}

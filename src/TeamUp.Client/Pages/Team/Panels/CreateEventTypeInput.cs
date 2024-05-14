using System.ComponentModel.DataAnnotations;

namespace TeamUp.Client.Pages.Team.Panels;

public sealed class CreateEventTypeInput
{
	[Required]
	public required string Name { get; set; }

	[Required]
	public required string Description { get; set; }

	public IDictionary<string, string[]>? Errors { get; set; }

	public void Reset()
	{
		Name = "";
		Description = "";
		Errors = null;
	}
}

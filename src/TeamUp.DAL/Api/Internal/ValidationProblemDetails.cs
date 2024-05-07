using System.Text.Json.Serialization;

namespace TeamUp.DAL.Api.Internal;

internal sealed class ValidationProblemDetails : ProblemDetails
{
	public ValidationProblemDetails() : this(new Dictionary<string, string[]>(StringComparer.Ordinal))
	{
	}

	public ValidationProblemDetails(IDictionary<string, string[]> errors) : this(new Dictionary<string, string[]>(errors ?? throw new ArgumentNullException(nameof(errors)), StringComparer.Ordinal))
	{
	}

	private ValidationProblemDetails(Dictionary<string, string[]> errors)
	{
		Title = "One or more validation errors occurred.";
		Errors = errors;
	}

	[JsonPropertyName("errors")]
	public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>(StringComparer.Ordinal);
}

using System.ComponentModel.DataAnnotations;

namespace TeamUp.Contracts.Events;

public sealed record UpsertEventReplyRequest
{
	public required ReplyType ReplyType { get; init; }

	[DataType(DataType.Text)]
	public required string Message { get; init; }
}

using TeamUp.Contracts.Events;

namespace TeamUp.Client.Pages.Event.Panels;

public class UpsertEventReplyInput
{
	public EventSlimResponse? Event { get; set; }

	public ReplyType? ReplyType { get; set; } = null;

	public string Message { get; set; } = "";

	public IDictionary<string, string[]>? Errors { get; set; }

	public void Reset()
	{
		Event = null;
		ReplyType = null;
		Message = "";
		Errors = null;
	}
}

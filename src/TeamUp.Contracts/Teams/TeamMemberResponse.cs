﻿using TeamUp.Contracts.Users;

namespace TeamUp.Contracts.Teams;

public sealed class TeamMemberResponse
{
	public required TeamMemberId Id { get; init; }
	public required UserId UserId { get; init; }
	public required string Nickname { get; set; }
	public required TeamRole Role { get; set; }
}

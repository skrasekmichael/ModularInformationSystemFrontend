using System.Net;

using RailwayResult;

namespace TeamUp.Contracts;

public sealed record ApiUnexpectedError(string Key, string Message) : Error(Key, Message);

public sealed record ApiError(string Key, string Message, HttpStatusCode HttpStatusCode) : Error(Key, Message);

public sealed record ApiValidationError(string Key, string Message, IDictionary<string, string[]> Errors) : Error(Key, Message);

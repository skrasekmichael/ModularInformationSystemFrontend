using RailwayResult;

namespace TeamUp.DAL;

public sealed record DataAccessError(string Key, string Message) : Error(Key, Message);

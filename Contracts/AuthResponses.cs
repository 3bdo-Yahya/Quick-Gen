namespace Quick_Gen.Contracts;

public sealed class AuthResponse
{
    public required string Token { get; init; }

    public required DateTimeOffset ExpiresAtUtc { get; init; }

    public required UserSummary User { get; init; }
}

public sealed class UserSummary
{
    public required string Id { get; init; }

    public required string Email { get; init; }

    public string? FullName { get; init; }

    public required IReadOnlyList<string> Roles { get; init; }
}

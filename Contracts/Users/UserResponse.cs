namespace Quick_Gen.Contracts.Users;

public sealed record UserResponse(
    string Id,
    string FullName,
    string Email,
    string Role,
    bool IsBanned,
    string CreatedAt
);
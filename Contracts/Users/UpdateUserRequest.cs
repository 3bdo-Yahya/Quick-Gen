namespace Quick_Gen.Contracts.Users;

public sealed record UpdateUserRequest(
    string FullName,
    string Email
);
namespace Quick_Gen.Contracts.Users;

public sealed record CreateUserRequest(
    string FullName,
    string Email,
    string Password
);
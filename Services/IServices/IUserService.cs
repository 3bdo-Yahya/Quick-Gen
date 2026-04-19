using Quick_Gen.Contracts.Users;

namespace Quick_Gen.Services.IServices;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<UserResponse?> GetByIdAsync(string id);
    Task<UserResponse> CreateAsync(CreateUserRequest request);
    Task<UserResponse?> UpdateAsync(string id, UpdateUserRequest request);
    Task<bool> DeleteAsync(string id);
}
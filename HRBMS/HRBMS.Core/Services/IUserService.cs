using HRBMS.Core.Entities;

namespace HRBMS.Core.Services;

public interface IUserService
{
    Task<User> RegisterGuestAsync(string username, string email, string password, CancellationToken cancellationToken);
    Task<User> RegisterStaffAsync(string username, string email, string password, IEnumerable<string> roles, CancellationToken cancellationToken);
    Task<User?> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> GetRoleNamesAsync(int userId, CancellationToken cancellationToken);
    Task AssignRoleAsync(int userId, string roleName, CancellationToken cancellationToken);
}
using HRBMS.Core.Entities;
using HRBMS.Core.Services;
using HRBMS.Infrastructure.Data;
using HRBMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRBMS.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly HrbmsDbContext _dbContext;
    private readonly UserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(HrbmsDbContext dbContext, UserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> RegisterGuestAsync(string username, string email, string password, CancellationToken cancellationToken)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Username == username, cancellationToken))
        {
            throw new InvalidOperationException("Username already exists");
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = _passwordHasher.HashPassword(password)
        };

        await _userRepository.AddAsync(user, new[] { "Guest" }, cancellationToken);
        return user;
    }

    public async Task<User> RegisterStaffAsync(string username, string email, string password, IEnumerable<string> roles, CancellationToken cancellationToken)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Username == username, cancellationToken))
        {
            throw new InvalidOperationException("Username already exists");
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = _passwordHasher.HashPassword(password)
        };

        await _userRepository.AddAsync(user, roles, cancellationToken);
        return user;
    }

    public async Task<User?> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        if (user == null) return null;
        return _passwordHasher.VerifyPassword(password, user.PasswordHash) ? user : null;
    }

    public Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken)
    {
        return _userRepository.GetByIdAsync(userId, cancellationToken);
    }

    public Task<IReadOnlyList<string>> GetRoleNamesAsync(int userId, CancellationToken cancellationToken)
    {
        return _userRepository.GetRoleNamesAsync(userId, cancellationToken);
    }

    public Task AssignRoleAsync(int userId, string roleName, CancellationToken cancellationToken)
    {
        return _userRepository.AssignRoleAsync(userId, roleName, cancellationToken);
    }
}
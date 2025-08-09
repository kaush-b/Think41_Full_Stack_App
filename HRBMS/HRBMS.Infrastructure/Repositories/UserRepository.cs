using HRBMS.Core.Entities;
using HRBMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRBMS.Infrastructure.Repositories;

public class UserRepository
{
    private readonly HrbmsDbContext _db;

    public UserRepository(HrbmsDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return _db.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _db.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task AddAsync(User user, IEnumerable<string> roles, CancellationToken cancellationToken)
    {
        var normalizedRoles = roles.Select(r => r.Trim()).Where(r => !string.IsNullOrWhiteSpace(r)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var roleEntities = await _db.Roles.Where(r => normalizedRoles.Contains(r.Name)).ToListAsync(cancellationToken);
        var missing = normalizedRoles.Except(roleEntities.Select(r => r.Name), StringComparer.OrdinalIgnoreCase).ToArray();
        foreach (var roleName in missing)
        {
            var role = new Role { Name = roleName };
            roleEntities.Add(role);
            _db.Roles.Add(role);
        }

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        foreach (var role in roleEntities)
        {
            _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignRoleAsync(int userId, string roleName, CancellationToken cancellationToken)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken) 
                   ?? _db.Roles.Add(new Role { Name = roleName }).Entity;
        if (!await _db.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == role.Id, cancellationToken))
        {
            _db.UserRoles.Add(new UserRole { UserId = userId, RoleId = role.Id });
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public Task<IReadOnlyList<string>> GetRoleNamesAsync(int userId, CancellationToken cancellationToken)
    {
        return _db.UserRoles.Where(ur => ur.UserId == userId).Include(ur => ur.Role)
            .Select(ur => ur.Role!.Name).ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<string>)t.Result, cancellationToken);
    }
}
using System.Security.Claims;
using FluentAssertions;
using Xunit;

namespace HRBMS.Tests;

public class AuthorizationTests
{
    [Fact]
    public void StaffOrAdmin_Allows_Staff()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "Staff") }, "TestAuth"));
        var isAllowed = user.IsInRole("Staff") || user.IsInRole("Admin");
        isAllowed.Should().BeTrue();
    }

    [Fact]
    public void StaffOrAdmin_Denies_Guest()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "Guest") }, "TestAuth"));
        var isAllowed = user.IsInRole("Staff") || user.IsInRole("Admin");
        isAllowed.Should().BeFalse();
    }
}
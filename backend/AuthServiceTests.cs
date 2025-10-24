using Xunit;
using Moq;
using UserManagementAPI.Services;
using UserManagementAPI.Data;

namespace UserManagementAPI.Tests.UnitTesting;

public class AuthServiceTests
{
    private readonly AuthService _authService;
    private readonly Mock<UserRepository> _userRepoMock;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<UserRepository>(null);  // Pass null or mock for connection string
        _authService = new AuthService(_userRepoMock.Object);
    }

    [Fact]
    public void Login_ValidUser_ReturnsToken()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        _userRepoMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "Teacher"
        });

        // Act
        var token = _authService.Login(email, password).Result;

        // Assert
        Assert.NotNull(token);
        Assert.IsType<string>(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void Login_InvalidUser_ReturnsNull()
    {
        // Arrange
        var email = "notfound@example.com";
        var password = "password123";
        _userRepoMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync((User)null);

        // Act
        var token = _authService.Login(email, password).Result;

        // Assert
        Assert.Null(token);
    }
}

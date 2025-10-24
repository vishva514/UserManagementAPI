using FluentAssertions;
using Microsoft.Extensions.Configuration;
using UserManagementAPI.Models;
using UserManagementAPI.Services;
using Xunit;

namespace UserManagementAPI.Tests.UnitTesting;

public class AuthServiceTests
{
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        // Setup mock configuration
        var inMemorySettings = new Dictionary<string, string>
        {
            {"JwtSettings:SecretKey", "afgsjkshjdndhuydwkkkhuiresdfgvcxxzasqplmjhydsxzaqwertyuimbvc"},
            {"JwtSettings:Issuer", "UserManagementAPI"},
            {"JwtSettings:Audience", "UserManagementAPIUsers"},
            {"JwtSettings:ExpiryInMinutes", "60"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _authService = new AuthService(configuration);
    }

    [Fact]
    public void HashPassword_WithValidPassword_ReturnsHashedPassword()
    {
        // Arrange
        var password = "TestPassword123";

        // Act
        var hashedPassword = _authService.HashPassword(password);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        hashedPassword.Should().NotBe(password);
        hashedPassword.Should().StartWith("$2"); // BCrypt hash format
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "TestPassword123";
        var hashedPassword = _authService.HashPassword(password);

        // Act
        var result = _authService.VerifyPassword(password, hashedPassword);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var correctPassword = "TestPassword123";
        var wrongPassword = "WrongPassword456";
        var hashedPassword = _authService.HashPassword(correctPassword);

        // Act
        var result = _authService.VerifyPassword(wrongPassword, hashedPassword);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GenerateJwtToken_WithValidUser_ReturnsToken()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            Name = "Test User",
            Designation = "Teacher",
            DateOfBirth = new DateTime(1990, 1, 1),
            PasswordHash = "hashedpassword"
        };

        // Act
        var token = _authService.GenerateJwtToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Length.Should().Be(3); // JWT has 3 parts
    }
}

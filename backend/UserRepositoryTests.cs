using Xunit;
using Moq;
using System.Threading.Tasks;
using UserManagementAPI.Data;
using UserManagementAPI.Models;
using System.Collections.Generic;
namespace UserManagementAPI.Tests.UnitTesting;
public class UserRepositoryTests
{
    private readonly UserRepository _userRepository;

    // You may need to mock or use in-memory DB based on implementation
    public UserRepositoryTests()
    {
        // If your UserRepository uses concrete SQL connections internally,
        // replace with a test double or refactor for DI to mock dependencies

        // For demonstration, example initializing with test connection string:
        _userRepository = new UserRepository("TestConnectionString");
    }

    [Fact]
    public async Task GetUserByEmailAsync_UserExists_ReturnsUser()
    {
        // Arrange
        var email = "existinguser@example.com";

        // Act
        var user = await _userRepository.GetUserByEmailAsync(email);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public async Task GetUserByEmailAsync_UserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var email = "nonexistent@example.com";

        // Act
        var user = await _userRepository.GetUserByEmailAsync(email);

        // Assert
        Assert.Null(user);
    }

    [Fact]
    public async Task CreateUserAsync_ValidUser_CompletesSuccessfully()
    {
        // Arrange
        var newUser = new User
        {
            Email = "newuser@example.com",
            Name = "New User",
            PasswordHash = "hashedpassword",
            Role = "Student",
            DateOfBirth = "2001-01-01",
            Designation = "Student"
        };

        // Act & Assert
        await _userRepository.CreateUserAsync(newUser); // Should complete without exceptions
    }

    // Add tests for UpdateUserAsync, DeleteUserAsync as needed
}

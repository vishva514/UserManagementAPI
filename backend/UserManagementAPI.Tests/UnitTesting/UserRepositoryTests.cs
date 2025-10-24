using FluentAssertions;
using UserManagementAPI.Data;
using UserManagementAPI.Models;
using Xunit;

namespace UserManagementAPI.Tests.UnitTesting;

public class UserRepositoryTests
{
    private readonly string _connectionString;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        // Use your test database connection string
        _connectionString = "Server=.;Database=SCHOOLMANG;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
        _userRepository = new UserRepository(_connectionString);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsListOfUsers()
    {
        // Act
        var users = await _userRepository.GetAllUsersAsync();

        // Assert
        users.Should().NotBeNull();
        users.Should().BeOfType<List<User>>();
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithExistingEmail_ReturnsUser()
    {
        // Arrange - First add a test user
        var testUser = new User
        {
            Name = "Unit Test User",
            Email = $"unittest{Guid.NewGuid()}@test.com",
            DateOfBirth = new DateTime(1990, 1, 1),
            Designation = "Teacher",
            PasswordHash = "hashedpassword"
        };
        await _userRepository.AddUserAsync(testUser);

        // Act
        var retrievedUser = await _userRepository.GetUserByEmailAsync(testUser.Email);

        // Assert
        retrievedUser.Should().NotBeNull();
        retrievedUser.Email.Should().Be(testUser.Email);
        retrievedUser.Name.Should().Be(testUser.Name);

        // Cleanup
        await _userRepository.DeleteUserAsync(testUser.Email);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithNonExistentEmail_ReturnsNull()
    {
        // Act
        var user = await _userRepository.GetUserByEmailAsync("nonexistent@test.com");

        // Assert
        user.Should().BeNull();
    }

    [Fact]
    public async Task AddUser_ThenDelete_WorksCorrectly()
    {
        // Arrange
        var testUser = new User
        {
            Name = "Add Delete Test",
            Email = $"adddelete{Guid.NewGuid()}@test.com",
            DateOfBirth = new DateTime(1995, 5, 15),
            Designation = "Student",
            PasswordHash = "hashedpassword"
        };

        // Act - Add
        await _userRepository.AddUserAsync(testUser);
        var addedUser = await _userRepository.GetUserByEmailAsync(testUser.Email);

        // Assert - User was added
        addedUser.Should().NotBeNull();
        addedUser.Email.Should().Be(testUser.Email);

        // Act - Delete
        await _userRepository.DeleteUserAsync(testUser.Email);
        var deletedUser = await _userRepository.GetUserByEmailAsync(testUser.Email);

        // Assert - User was deleted
        deletedUser.Should().BeNull();
    }
}

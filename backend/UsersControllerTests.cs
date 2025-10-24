using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserManagementAPI.Controllers;
using UserManagementAPI.Services;
using UserManagementAPI.Data;
using UserManagementAPI.Models;  // Assuming models namespace
namespace UserManagementAPI.Tests.UnitTesting;
public class UsersControllerTests
{
    private readonly UsersController _controller;
    private readonly Mock<AuthService> _authServiceMock;
    private readonly Mock<UserRepository> _userRepositoryMock;

    public UsersControllerTests()
    {
        _authServiceMock = new Mock<AuthService>();
        _userRepositoryMock = new Mock<UserRepository>(null); // Null for connection string

        _controller = new UsersController(_authServiceMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsOk()
    {
        // Arrange
        var newUser = new UserRegisterDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            Name = "Test User",
            DateOfBirth = "2000-01-01",
            Designation = "Student"
        };

        _userRepositoryMock.Setup(repo => repo.UserExistsAsync(newUser.Email)).ReturnsAsync(false);
        _userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Register(newUser);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Register_ExistingUser_ReturnsBadRequest()
    {
        var newUser = new UserRegisterDto { Email = "existing@example.com", Password = "pass" };
        _userRepositoryMock.Setup(repo => repo.UserExistsAsync(newUser.Email)).ReturnsAsync(true);

        var result = await _controller.Register(newUser);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("User already exists.", badRequest.Value);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var loginDto = new UserLoginDto { Email = "test@example.com", Password = "Password123!" };
        var expectedToken = "jwt-token-string";

        _authServiceMock.Setup(auth => auth.Login(loginDto.Email, loginDto.Password)).ReturnsAsync(expectedToken);

        var actionResult = await _controller.Login(loginDto);

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        dynamic obj = okResult.Value;
        Assert.Equal(expectedToken, (string)obj.token);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var loginDto = new UserLoginDto { Email = "wrong@example.com", Password = "badpass" };

        _authServiceMock.Setup(auth => auth.Login(loginDto.Email, loginDto.Password)).ReturnsAsync((string)null);

        var actionResult = await _controller.Login(loginDto);

        Assert.IsType<UnauthorizedResult>(actionResult);
    }

    // Add more tests for GetUsers, UpdateUser, DeleteUser etc.
}

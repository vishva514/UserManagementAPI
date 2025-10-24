using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserManagementAPI.Controllers;
using UserManagementAPI.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UserManagementAPI.Data;
using UserManagementAPI.Services;
using Xunit;

namespace UserManagementAPI.Tests.UnitTesting;

public class UsersControllerTests
{
    private readonly UsersController _controller;
    private readonly UserRepository _userRepository;
    private readonly AuthService _authService;

    public UsersControllerTests()
    {
        // Setup configuration
        var inMemorySettings = new Dictionary<string, string>
        {
            {"ConnectionStrings:DefaultConnection", "Server=.;Database=SCHOOLMANG;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"},
            {"JwtSettings:SecretKey", "afgsjkshjdndhuydwkkkhuiresdfgvcxxzasqplmjhydsxzaqwertyuimbvc"},
            {"JwtSettings:Issuer", "UserManagementAPI"},
            {"JwtSettings:Audience", "UserManagementAPIUsers"},
            {"JwtSettings:ExpiryInMinutes", "60"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        _userRepository = new UserRepository(connectionString);
        _authService = new AuthService(configuration);
        _controller = new UsersController(_userRepository, _authService);
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOk()
    {
        // Arrange
        var registerDto = new UsersController.UserRegisterDto
        {
            Name = "Unit Test Register",
            Email = $"register{Guid.NewGuid()}@test.com",
            Password = "password123",
            DateOfBirth = new DateTime(1990, 1, 1),
            Designation = "Teacher"
        };

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().Be("User registration successful.");

        // Cleanup
        await _userRepository.DeleteUserAsync(registerDto.Email);
    }

    [Fact]
    public async Task Register_WithInvalidDesignation_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new UsersController.UserRegisterDto
        {
            Name = "Invalid Designation Test",
            Email = $"invalid{Guid.NewGuid()}@test.com",
            Password = "password123",
            DateOfBirth = new DateTime(1990, 1, 1),
            Designation = "Manager" // Invalid - only Teacher/Student allowed
        };

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        // Arrange - First register a user
        var email = $"login{Guid.NewGuid()}@test.com";
        var password = "password123";

        var registerDto = new UsersController.UserRegisterDto
        {
            Name = "Login Test User",
            Email = email,
            Password = password,
            DateOfBirth = new DateTime(1990, 1, 1),
            Designation = "Student"
        };
        await _controller.Register(registerDto);

        var loginDto = new UsersController.LoginDto
        {
            Email = email,
            Password = password
        };

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().NotBeNull();

        // Cleanup
        await _userRepository.DeleteUserAsync(email);
    }
}

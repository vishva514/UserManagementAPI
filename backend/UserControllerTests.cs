using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace UserManagementAPI.Tests.IntegrationTests;

[Collection("Sequential")]
public class UsersControllerIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersControllerIntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<(string token, string email, string role)> RegisterAndLoginAsync(string designation = "Teacher")
    {
        var uniqueEmail = $"user{Guid.NewGuid()}@test.com";

        // Register
        var registerData = new
        {
            name = $"Test {designation}",
            dateOfBirth = "1990-01-01",
            designation = designation,
            email = uniqueEmail,
            password = "password123"
        };
        await _client.PostAsJsonAsync("/api/users/register", registerData);

        // Login
        var loginData = new
        {
            email = uniqueEmail,
            password = "password123"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/users/login", loginData);
        var content = await loginResponse.Content.ReadAsStringAsync();
        var json = JObject.Parse(content);

        return (json["token"].ToString(), uniqueEmail, json["role"].ToString());
    }

    #region Registration Tests

    [Fact]
    public async Task Register_WithValidTeacherData_ReturnsOk()
    {
        // Arrange
        var uniqueEmail = $"teacher{Guid.NewGuid()}@test.com";
        var registerData = new
        {
            name = "New Teacher",
            dateOfBirth = "1985-05-15",
            designation = "Teacher",
            email = uniqueEmail,
            password = "SecurePass123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", registerData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("User registration successful");
    }

    [Fact]
    public async Task Register_WithValidStudentData_ReturnsOk()
    {
        // Arrange
        var uniqueEmail = $"student{Guid.NewGuid()}@test.com";
        var registerData = new
        {
            name = "New Student",
            dateOfBirth = "2000-03-20",
            designation = "Student",
            email = uniqueEmail,
            password = "SecurePass123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", registerData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var uniqueEmail = $"duplicate{Guid.NewGuid()}@test.com";
        var registerData = new
        {
            name = "User One",
            dateOfBirth = "1990-01-01",
            designation = "Teacher",
            email = uniqueEmail,
            password = "password123"
        };

        // Act - Register first time
        await _client.PostAsJsonAsync("/api/users/register", registerData);

        // Act - Try to register again with same email
        var response = await _client.PostAsJsonAsync("/api/users/register", registerData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Email is already registered");
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("Manager")]
    [InlineData("Invalid")]
    public async Task Register_WithInvalidDesignation_ReturnsBadRequest(string invalidDesignation)
    {
        // Arrange
        var uniqueEmail = $"invalid{Guid.NewGuid()}@test.com";
        var registerData = new
        {
            name = "Invalid User",
            dateOfBirth = "1990-01-01",
            designation = invalidDesignation,
            email = uniqueEmail,
            password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", registerData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Designation must be either 'Teacher' or 'Student'");
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokenAndUserInfo()
    {
        // Arrange - Register a user first
        var uniqueEmail = $"loginuser{Guid.NewGuid()}@test.com";
        var registerData = new
        {
            name = "Login Test User",
            dateOfBirth = "1990-01-01",
            designation = "Teacher",
            email = uniqueEmail,
            password = "password123"
        };
        await _client.PostAsJsonAsync("/api/users/register", registerData);

        var loginData = new
        {
            email = uniqueEmail,
            password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/login", loginData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(content);

        json["token"].Should().NotBeNull();
        json["token"].ToString().Should().NotBeEmpty();
        json["role"].ToString().Should().Be("Teacher");
        json["name"].ToString().Should().Be("Login Test User");
        json["email"].ToString().Should().Be(uniqueEmail);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        // Arrange - Register a user first
        var uniqueEmail = $"wrongpass{Guid.NewGuid()}@test.com";
        var registerData = new
        {
            name = "Wrong Pass User",
            dateOfBirth = "1990-01-01",
            designation = "Teacher",
            email = uniqueEmail,
            password = "correctpassword"
        };
        await _client.PostAsJsonAsync("/api/users/register", registerData);

        var loginData = new
        {
            email = uniqueEmail,
            password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/login", loginData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid email or password");
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ReturnsUnauthorized()
    {
        // Arrange
        var loginData = new
        {
            email = "nonexistent@test.com",
            password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/login", loginData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Get Users Tests

    [Fact]
    public async Task GetUsers_WithTeacherToken_ReturnsUserList()
    {
        // Arrange
        var (token, _, _) = await RegisterAndLoginAsync("Teacher");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var users = JArray.Parse(content);

        users.Should().NotBeNull();
        users.Count.Should().BeGreaterThan(0);

        // Verify user structure (should NOT contain PasswordHash)
        var firstUser = users[0];
        firstUser["id"].Should().NotBeNull();
        firstUser["name"].Should().NotBeNull();
        firstUser["email"].Should().NotBeNull();
        firstUser["designation"].Should().NotBeNull();
        firstUser["passwordHash"].Should().BeNull(); // Password should NOT be exposed
    }

    [Fact]
    public async Task GetUsers_WithStudentToken_ReturnsUserList()
    {
        // Arrange
        var (token, _, _) = await RegisterAndLoginAsync("Student");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUsers_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Update User Tests

    [Fact]
    public async Task UpdateUser_AsTeacher_ReturnsOk()
    {
        // Arrange - Register two users: one teacher and one to update
        var (teacherToken, _, _) = await RegisterAndLoginAsync("Teacher");
        var (_, targetEmail, _) = await RegisterAndLoginAsync("Student");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", teacherToken);

        var updateData = new
        {
            email = targetEmail,
            name = "Updated Name",
            dateOfBirth = "1995-06-15",
            password = "newpassword123"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users", updateData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("User updated successfully");

        // Verify the update by getting user details
        var getResponse = await _client.GetAsync("/api/users");
        var users = await getResponse.Content.ReadAsStringAsync();
        users.Should().Contain("Updated Name");
    }

    [Fact]
    public async Task UpdateUser_AsStudent_ReturnsForbidden()
    {
        // Arrange - Register student who tries to update
        var (studentToken, _, _) = await RegisterAndLoginAsync("Student");
        var (_, targetEmail, _) = await RegisterAndLoginAsync("Teacher");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", studentToken);

        var updateData = new
        {
            email = targetEmail,
            name = "Unauthorized Update",
            dateOfBirth = "1990-01-01",
            password = "newpass"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users", updateData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUser_NonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var (teacherToken, _, _) = await RegisterAndLoginAsync("Teacher");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", teacherToken);

        var updateData = new
        {
            email = "nonexistent@test.com",
            name = "Updated Name",
            dateOfBirth = "1990-01-01",
            password = "newpass"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users", updateData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("User not found");
    }

    #endregion

    #region Delete User Tests

    [Fact]
    public async Task DeleteUser_AsTeacher_ReturnsOk()
    {
        // Arrange
        var (teacherToken, _, _) = await RegisterAndLoginAsync("Teacher");
        var (_, targetEmail, _) = await RegisterAndLoginAsync("Student");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", teacherToken);

        // Act
        var response = await _client.DeleteAsync($"/api/users/{targetEmail}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("User deleted successfully");
    }

    [Fact]
    public async Task DeleteUser_AsStudent_ReturnsForbidden()
    {
        // Arrange
        var (_, teacherEmail, _) = await RegisterAndLoginAsync("Teacher");
        var (studentToken, _, _) = await RegisterAndLoginAsync("Student");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", studentToken);

        // Act
        var response = await _client.DeleteAsync($"/api/users/{teacherEmail}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteUser_NonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var (teacherToken, _, _) = await RegisterAndLoginAsync("Teacher");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", teacherToken);

        // Act
        var response = await _client.DeleteAsync("/api/users/nonexistent@test.com");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUser_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var (_, targetEmail, _) = await RegisterAndLoginAsync("Student");

        // Act (no token set)
        var response = await _client.DeleteAsync($"/api/users/{targetEmail}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region End-to-End Workflow Tests

    [Fact]
    public async Task CompleteWorkflow_RegisterLoginUpdateDelete_WorksCorrectly()
    {
        var uniqueEmail = $"workflow{Guid.NewGuid()}@test.com";

        // 1. Register
        var registerData = new
        {
            name = "Workflow User",
            dateOfBirth = "1990-01-01",
            designation = "Teacher",
            email = uniqueEmail,
            password = "password123"
        };
        var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerData);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 2. Login
        var loginData = new { email = uniqueEmail, password = "password123" };
        var loginResponse = await _client.PostAsJsonAsync("/api/users/login", loginData);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginJson = JObject.Parse(loginContent);
        var token = loginJson["token"].ToString();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // 3. Get Users
        var getUsersResponse = await _client.GetAsync("/api/users");
        getUsersResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 4. Update
        var updateData = new
        {
            email = uniqueEmail,
            name = "Updated Workflow User",
            dateOfBirth = "1990-01-01",
            password = "newpassword123"
        };
        var updateResponse = await _client.PutAsJsonAsync("/api/users", updateData);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 5. Delete
        var deleteResponse = await _client.DeleteAsync($"/api/users/{uniqueEmail}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}


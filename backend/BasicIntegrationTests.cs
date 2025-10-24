using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace UserManagementAPI.Tests.IntegrationTests;

public class BasicIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BasicIntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Test1_Register_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var uniqueEmail = $"user{Guid.NewGuid()}@test.com";
        var registerData = new
        {
            name = "Test User",
            dateOfBirth = "1995-03-20",
            designation = "Teacher",
            email = uniqueEmail,
            password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", registerData);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Contain("User registration successful");
    }

    [Fact]
    public async Task Test2_Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange - Register a user first
        var uniqueEmail = $"login{Guid.NewGuid()}@test.com";
        var registerData = new
        {
            name = "Login User",
            dateOfBirth = "1990-01-01",
            designation = "Student",
            email = uniqueEmail,
            password = "password123"
        };
        await _client.PostAsJsonAsync("/api/users/register", registerData);

        // Login with same credentials
        var loginData = new
        {
            email = uniqueEmail,
            password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/login", loginData);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = JObject.Parse(content);
        json["token"].Should().NotBeNull();
        json["role"].ToString().Should().Be("Student");
        json["email"].ToString().Should().Be(uniqueEmail);
    }

    [Fact]
    public async Task Test3_GetAllUsers_WithValidToken_ReturnsUserList()
    {
        // Arrange - Register and login to get token
        var uniqueEmail = $"getusers{Guid.NewGuid()}@test.com";
        var registerData = new
        {
            name = "Get Users Test",
            dateOfBirth = "1990-01-01",
            designation = "Teacher",
            email = uniqueEmail,
            password = "password123"
        };
        await _client.PostAsJsonAsync("/api/users/register", registerData);

        var loginData = new { email = uniqueEmail, password = "password123" };
        var loginResponse = await _client.PostAsJsonAsync("/api/users/login", loginData);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginJson = JObject.Parse(loginContent);
        var token = loginJson["token"].ToString();

        // Set authorization header
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>(content);
        users.Should().NotBeNull();
        users.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Test4_GetAllUsers_WithoutToken_ReturnsUnauthorized()
    {
        // Act - Try to get users without authentication
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace UserManagementAPI.Tests.IntegrationTests;

public class SmokeTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SmokeTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task API_IsRunning_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/api/users");
        response.Should().NotBeNull();
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_Endpoint_Responds()
    {
        var userData = new
        {
            name = "Smoke Test User",
            dateOfBirth = "1990-01-01",
            designation = "Teacher",
            email = $"smoke{Guid.NewGuid()}@test.com",
            password = "test123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", userData);
        response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Login_Endpoint_Responds()
    {
        // Arrange
        var loginData = new
        {
            email = "test@test.com",
            password = "password123"
        };
        var response = await _client.PostAsJsonAsync("/api/users/login", loginData);
        response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Database_Connection_Works()
    {
        var userData = new
        {
            name = "DB Test User",
            dateOfBirth = "1990-01-01",
            designation = "Student",
            email = $"dbtest{Guid.NewGuid()}@test.com",
            password = "test123"
        };
        var response = await _client.PostAsJsonAsync("/api/users/register", userData);
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}

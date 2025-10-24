using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace UserManagementAPI.Tests.IntegrationTests;

public class AuthControllerTests : IClassFixture<TestWebApplicationFactory>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost")
        });
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOk()
    {
        // Arrange
        var uniqueEmail = $"test{Guid.NewGuid()}@test.com";
        var registerData = new
        {
            name = "Test User",
            dateOfBirth = "1995-03-20",
            designation = "Student",
            email = uniqueEmail,
            password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", registerData);

        // Debug
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Status: {response.StatusCode}, Content: {content}");

        // Assert
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            Assert.Fail($"Route not found. Registered routes might not include /api/users/register");
        }

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}

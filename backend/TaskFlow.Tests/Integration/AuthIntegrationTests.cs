using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace TaskFlow.Tests.Integration;

[Collection("Integration")]
public class AuthIntegrationTests
{
    private readonly TaskFlowFactory _factory;

    public AuthIntegrationTests(TaskFlowFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_ValidRequest_Returns201()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            name = "Test User",
            email = "auth-register@example.com",
            password = "Password123!"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var client = _factory.CreateClient();

        var reg = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            name = "Login User",
            email = "auth-login@example.com",
            password = "Password123!"
        });
        Assert.Equal(HttpStatusCode.Created, reg.StatusCode);

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "auth-login@example.com",
            password = "Password123!"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetProperty("success").GetBoolean());
        var token = body.GetProperty("data").GetProperty("token").GetString();
        Assert.False(string.IsNullOrEmpty(token));
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            name = "WrongPass User",
            email = "auth-wrongpass@example.com",
            password = "Password123!"
        });

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "auth-wrongpass@example.com",
            password = "WrongPassword"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace TaskFlow.Tests.Integration;

[Collection("Integration")]
public class TasksIntegrationTests
{
    private readonly TaskFlowFactory _factory;

    public TasksIntegrationTests(TaskFlowFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetTasks_Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/v1/projects/1/tasks");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateTask_Valid_Returns201()
    {
        var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client, "tasks-createtask@example.com");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var projectResponse = await client.PostAsJsonAsync("/api/v1/projects", new
        {
            name = "Integration Project",
            description = "For task integration test"
        });
        Assert.Equal(HttpStatusCode.Created, projectResponse.StatusCode);

        var projectBody = await projectResponse.Content.ReadFromJsonAsync<JsonElement>();
        var projectId = projectBody.GetProperty("data").GetProperty("id").GetInt32();

        var taskResponse = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/tasks", new
        {
            title = "Integration Task",
            description = "Created in test",
            priority = "High"
        });

        Assert.Equal(HttpStatusCode.Created, taskResponse.StatusCode);
        var taskBody = await taskResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Integration Task", taskBody.GetProperty("data").GetProperty("title").GetString());
    }

    private static async Task<string> RegisterAndLoginAsync(HttpClient client, string email)
    {
        await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            name = "Integration User",
            email,
            password = "Password123!"
        });

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email,
            password = "Password123!"
        });

        var body = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("data").GetProperty("token").GetString()!;
    }
}

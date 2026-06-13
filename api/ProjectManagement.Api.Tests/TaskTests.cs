using System.Net;
using System.Net.Http.Json;
using ProjectManagement.Api.Common;
using ProjectManagement.Api.Domain;
using Xunit;

namespace ProjectManagement.Api.Tests;

public class TaskTests : IClassFixture<TestAppFactory>
{
    private readonly HttpClient _client;

    public TaskTests(TestAppFactory factory)
    {
        _client = factory.CreateAuthenticatedClientAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task CreateTask_AcceptsStringPriority()
    {
        var projectId = await TestAppFactory.GetFirstProjectIdAsync(_client);

        var response = await _client.PostAsJsonAsync(
            $"/api/projects/{projectId}/tasks",
            new { title = "String priority task", priority = "Medium" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<Result<TaskItem>>(TestAppFactory.JsonOptions);
        Assert.True(body?.Success);
        Assert.Equal("String priority task", body?.Value?.Title);
        Assert.Equal(Priority.Medium, body?.Value?.Priority);
    }
}

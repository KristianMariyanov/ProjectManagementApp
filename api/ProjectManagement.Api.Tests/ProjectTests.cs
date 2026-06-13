using System.Net;
using System.Net.Http.Json;
using ProjectManagement.Api.Common;
using ProjectManagement.Api.Services;
using Xunit;

namespace ProjectManagement.Api.Tests;

public class ProjectTests : IClassFixture<TestAppFactory>
{
    private readonly HttpClient _client;

    public ProjectTests(TestAppFactory factory)
    {
        _client = factory.CreateAuthenticatedClientAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task CreateProject_ReturnsSerializableProject()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/projects",
            new { name = "New App", description = "Created by an integration test" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<Result<ProjectDto>>(TestAppFactory.JsonOptions);
        Assert.True(body?.Success);
        Assert.Equal("New App", body?.Value?.Name);
        Assert.Single(body?.Value?.Members ?? []);
    }
}

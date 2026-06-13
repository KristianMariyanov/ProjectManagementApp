using Xunit;

namespace ProjectManagement.Api.Tests;

public class AppBoots : IClassFixture<TestAppFactory>
{
    private readonly TestAppFactory _factory;

    public AppBoots(TestAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthCheck_ReturnsSuccess()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/projects");

        // 401 is expected because the app booted and reached authorization.
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

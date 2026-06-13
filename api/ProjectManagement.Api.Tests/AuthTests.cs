using System.Net;
using System.Net.Http.Json;
using ProjectManagement.Api.Auth;
using ProjectManagement.Api.Common;
using Xunit;

namespace ProjectManagement.Api.Tests;

public class AuthTests : IClassFixture<TestAppFactory>
{
    private readonly HttpClient _client;

    public AuthTests(TestAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ThenLogin_ReturnsTokens()
    {
        var registerReq = new RegisterRequest("test@example.com", "Password1!", "Test User");
        var registerResp = await _client.PostAsJsonAsync("/api/auth/register", registerReq);
        Assert.Equal(HttpStatusCode.Created, registerResp.StatusCode);

        var loginReq = new LoginRequest("test@example.com", "Password1!");
        var loginResp = await _client.PostAsJsonAsync("/api/auth/login", loginReq);
        Assert.Equal(HttpStatusCode.OK, loginResp.StatusCode);

        var body = await loginResp.Content.ReadFromJsonAsync<Result<TokenResponse>>();
        Assert.True(body?.Success);
        Assert.NotEmpty(body?.Value?.AccessToken ?? "");
        Assert.NotEmpty(body?.Value?.RefreshToken ?? "");
    }
}

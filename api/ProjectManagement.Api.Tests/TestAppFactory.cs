using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Api.Auth;
using ProjectManagement.Api.Common;
using ProjectManagement.Api.Data;
using ProjectManagement.Api.Services;

namespace ProjectManagement.Api.Tests;

public class TestAppFactory : WebApplicationFactory<Program>
{
    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            var dbPath = Path.Combine(Path.GetTempPath(), $"project_management_test_{Guid.NewGuid():N}.db");
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));
        });
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest("alice@example.com", "Password1!"));
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<Result<TokenResponse>>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", body?.Value?.AccessToken);

        return client;
    }

    public static async Task<Guid> GetFirstProjectIdAsync(HttpClient client)
    {
        var response = await client.GetAsync("/api/projects");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<Result<List<ProjectDto>>>(JsonOptions);
        return body?.Value?.First().Id ?? throw new InvalidOperationException("Seed project was not created.");
    }
}

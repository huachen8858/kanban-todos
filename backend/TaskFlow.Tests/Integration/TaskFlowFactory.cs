using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Tests.Integration;

public class TaskFlowFactory : WebApplicationFactory<Program>
{
    private static readonly InMemoryDatabaseRoot DbRoot = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TaskFlowDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<TaskFlowDbContext>(options =>
                options.UseInMemoryDatabase("IntegrationTestDb", DbRoot));
        });
    }
}

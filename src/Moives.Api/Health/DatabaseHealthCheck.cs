using Microsoft.Extensions.Diagnostics.HealthChecks;
using Movies.Application.Database;

namespace Movies.Api.Health;

public class DatabaseHealthCheck : IHealthCheck
{
    public const string Name = "Database";

    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DatabaseHealthCheck(IDbConnectionFactory dbConnectionFactory, ILogger<DatabaseHealthCheck> logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        try
        {
            _ = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            const string errorMessage = "Database is unhealthy";
            return HealthCheckResult.Unhealthy(errorMessage, e);
        }
    }
}

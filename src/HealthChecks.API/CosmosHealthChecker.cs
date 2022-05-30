using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.API
{
    public class CosmosHealthChecker : IHealthCheck
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CosmosHealthChecker> _logger;

        public CosmosHealthChecker(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<CosmosHealthChecker> logger)
        {
            _serviceScopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                string collectionName = _configuration["Cosmos:Collection"];

                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                Database db = scope.ServiceProvider.GetRequiredService<Database>();
                Container container = db.GetContainer(collectionName);

                await container.ReadContainerAsync(cancellationToken: cancellationToken);
                return new HealthCheckResult(HealthStatus.Healthy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pinging Cosmos db");

                return new HealthCheckResult(
                    context.Registration.FailureStatus, "An unhealthy result.");
            }
        }
    }
}

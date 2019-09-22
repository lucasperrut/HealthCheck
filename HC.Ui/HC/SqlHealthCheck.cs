using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HC.Ui.HC
{
    public class SqlHealthCheck : IHealthCheck
    {
        private readonly IDbConnection _connection;

        public SqlHealthCheck(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _connection.Open();
                _connection.Close();
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Sql fora de serviço", ex));
            }

            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
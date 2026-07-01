using RecipeOrganizer.Domain.Entity;
using RecipeOrganizer.Domain.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOrganizer.Infrastructure.Services
{
    public class HealthService : IHealthService
    {
        private readonly IConfiguration _configuration;

        private readonly log4net.ILog _Logger = log4net.LogManager.GetLogger(typeof(HealthService));

        public HealthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<HealthResponse> GetHealthAsync()
        {
            try
            {
                return new HealthResponse
                {
                    Status = "Healthy",
                    ServiceName = "Employee Sync API",
                    ServerTime = DateTime.UtcNow,
                    ResponseCode = 200,
                    ResponseMessage = "OK"
                };
            }
            catch (Exception ex)
            {
                _Logger.Error("HealthService - GetHealthAsync", ex);

                return new HealthResponse
                {
                    Status = "Unhealthy",
                    ResponseCode = 500,
                    ResponseMessage = ex.Message
                };
            }
        }
    }
}

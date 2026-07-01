using Microsoft.AspNetCore.Mvc;
using RecipeOrganizer.Domain.Entity;
using RecipeOrganizer.Domain.Services;
using System.Net;

namespace RecipeOrganizer.API.Controllers
{
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class HealthCheckController : Controller
    {
        private readonly log4net.ILog _Logger = log4net.LogManager.GetLogger(typeof(HealthCheckController));



        private readonly IHealthService _healthService;

        public HealthCheckController(IHealthService healthService)
        {
            _healthService = healthService;
        }

        /// <summary>
        /// API Health Check
        /// </summary>
        [HttpGet]
        [Route("Check")]
        [ProducesResponseType(typeof(HealthResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Check()
        {
            try
            {
                HealthResponse response = await _healthService.GetHealthAsync();

                if (response == null)
                {
                    _Logger.Error("HealthController - Check - Response Null");
                    return StatusCode(500);
                }

                _Logger.Info("Health API executed successfully");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _Logger.Error("HealthController - Check", ex);
                return StatusCode(500);
            }
        }
    }
}

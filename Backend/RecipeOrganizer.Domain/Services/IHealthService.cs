using RecipeOrganizer.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOrganizer.Domain.Services
{
    public interface IHealthService
    {
        Task<HealthResponse> GetHealthAsync();
    }
}

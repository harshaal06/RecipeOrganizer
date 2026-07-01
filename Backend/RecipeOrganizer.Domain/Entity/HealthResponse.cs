using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOrganizer.Domain.Entity
{
    public class HealthResponse
    {
        public string Status { get; set; }
        public string ServiceName { get; set; }
        public DateTime ServerTime { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}

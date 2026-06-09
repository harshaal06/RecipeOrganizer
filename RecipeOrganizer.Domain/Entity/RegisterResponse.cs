using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOrganizer.Domain.Entity
{
    public class RegisterResponse
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string UserId { get; set; }
    }
}

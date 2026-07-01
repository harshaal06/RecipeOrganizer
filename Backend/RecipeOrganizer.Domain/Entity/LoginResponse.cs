using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOrganizer.Domain.Entity
{
    public class LoginResponse
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Role { get; set; } = new List<string>();
        public string Token { get; set; } = string.Empty;
    }
}

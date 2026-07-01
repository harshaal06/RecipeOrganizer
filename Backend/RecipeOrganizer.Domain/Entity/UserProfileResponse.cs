using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOrganizer.Domain.Entity
{
    public class UserProfileResponse : BaseResponse
    {
        public List<UserProfile> Users { get; set; } = new();

        public class UserProfile
        {
            public int Id { get; set; } = 0;
            public string UserId { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public List<string> Roles { get; set; } = new();
        }
    }
}

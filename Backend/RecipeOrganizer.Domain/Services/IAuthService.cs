using RecipeOrganizer.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOrganizer.Domain.Services
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<BaseResponse> AssignRoleAsync(AssignRoleRequest request);
        Task<UserProfileResponse> GetUserProfileAsync(UserProfileRequest request);
        Task<GetRolesResponse> GetUserRolesAsync(string userName);
        Task<GetRolesResponse> GetRolesAsync();
        Task<BaseResponse> CreateRolesAsync(CreateRoleRequest request, string createdBy);
        Task<BaseResponse> RemoveRoleAsync(string roleName);
        Task<BaseResponse> ChangePasswordAsync(string userName, ChangePasswordRequest request);
        //Task<BaseResponse> UpdateProfileAsync( string userId, UpdateProfileRequest request);
    }
}

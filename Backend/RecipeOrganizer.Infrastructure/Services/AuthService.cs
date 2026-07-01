using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using RecipeOrganizer.Domain.Entity;
using RecipeOrganizer.Domain.Services;
using RecipeOrganizer.Infrastructure.Query;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static RecipeOrganizer.Domain.Entity.UserProfileResponse;

namespace RecipeOrganizer.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public AuthService( IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("RecipeOrganizerDB");
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        RegisterResponse response = new RegisterResponse();

        SQLHelper sqlHelper = new SQLHelper();
        AuthQueryGenerator queryGenerator = new AuthQueryGenerator();

        try
        {
            string emailQuery = queryGenerator.GetUserQuery(email: request.Email);
            int emailCount = sqlHelper.ExecuteScalar(emailQuery, _connectionString);

            if (emailCount > 0)
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Email already exists";
                return response;
            }

            string userNameQuery = queryGenerator.GetUserQuery(userName: request.UserName);
            int userCount = sqlHelper.ExecuteScalar(userNameQuery, _connectionString);

            if (userCount > 0)
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Username already exists";
                return response;
            }

            User user = MapToUser(request);

            string insertUserQuery = queryGenerator.InsertUserQuery(user);
            int rowsAffected = sqlHelper.ExecuteNonQuery(insertUserQuery, _connectionString);

            if (rowsAffected <= 0)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = "Failed to register user.";
                return response;
            }

            await AssignRoleAsync(new AssignRoleRequest { UserName = user.UserName, RoleName = "User" });

            response.UserId = user.UserId;
            response.ResponseCode = 200;
            response.ResponseMessage = "Registration Successful";
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = "Internal Server Error";
        }
        finally
        {
            sqlHelper.CloseSqlConnection();
        }

        return response;
    }
    private User MapToUser(RegisterRequest request)
    {
        return new User
        {
            UserId = Guid.NewGuid().ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };
    }
    public async Task<BaseResponse> AssignRoleAsync(AssignRoleRequest request)
    {
        BaseResponse response = new BaseResponse();
        SQLHelper sqlHelper = new SQLHelper();
        AuthQueryGenerator queryGenerator = new AuthQueryGenerator();

        try
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.RoleName))
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "User Id and Role Name are required.";

                return response;
            }

            int roleId = 0;
            int userId = 0;

            string roleQuery = queryGenerator.GetUserIdAndRoleIdQuery(request.UserName, request.RoleName);

            using (MySqlDataReader reader = sqlHelper.ExecuteQuery(roleQuery, _connectionString))
            {
                if (reader.Read())
                {
                    roleId = SQLHelper.GetIntValue(reader, "RoleId");
                    userId = SQLHelper.GetIntValue(reader, "UserId");
                }
            }

            if (userId <= 0 || roleId <= 0)
            {
                response.ResponseCode = 404;
                response.ResponseMessage = "User or Role not found.";

                return response;
            }

            string roleAssignQuery = queryGenerator.AssignRoleQuery(userId, roleId);

            int rowsAffected = sqlHelper.ExecuteNonQuery(roleAssignQuery, _connectionString);

            if (rowsAffected > 0)
            {
                response.ResponseCode = 200;
                response.ResponseMessage = "Role assigned successfully.";
                response.RecordCount = rowsAffected;
            }
            else
            {
                response.ResponseCode = 500;
                response.ResponseMessage = "Failed to assign role.";
            }
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = ex.Message;
        }
        finally
        {
            sqlHelper.CloseSqlConnection();
        }

        return response;
    }
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        LoginResponse response = new();
        SQLHelper sqlHelper = new SQLHelper();
        AuthQueryGenerator queryGenerator = new AuthQueryGenerator();

        try
        {
            string query = queryGenerator.GetUserForLogin(request.UserNameOrEmail);

            User user = null;

            using (MySqlDataReader reader = sqlHelper.ExecuteQuery(query, _connectionString))
            {
                if (reader.Read())
                {
                    user = new User
                    {
                        Id = SQLHelper.GetIntValue(reader, "Id"),
                        UserId = SQLHelper.GetStringValue(reader, "EntityId"),
                        UserName = SQLHelper.GetStringValue(reader, "UserName"),
                        Email = SQLHelper.GetStringValue(reader, "Email"),
                        PasswordHash = SQLHelper.GetStringValue(reader, "PasswordHash")
                    };
                }
            }

            if (user == null)
            {
                response.ResponseCode = 401;
                response.ResponseMessage = "Invalid Username/Email";
                return response;
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isValidPassword)
            {
                response.ResponseCode = 401;
                response.ResponseMessage = "Invalid Password";
                return response;
            }

            List<string> roles = (await GetUserRolesAsync(user.UserName)).Roles;

            string token = GenerateToken(user, roles);

            response.ResponseCode = 200;
            response.ResponseMessage = "Login Successful";
            response.UserId = user.UserId;
            response.UserName = user.UserName;
            response.Email = user.Email;
            response.Role = roles;
            response.Token = token;            
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = ex.Message;
        }
        finally
        {
            sqlHelper.CloseSqlConnection();
        }
        return response;
    }
    public async Task<GetRolesResponse> GetUserRolesAsync(string userName)
    {
        GetRolesResponse response = new GetRolesResponse();
        SQLHelper sqlHelper = new SQLHelper();
        AuthQueryGenerator queryGenerator = new AuthQueryGenerator();

        try
        {
            string query = queryGenerator.GetRolesByUserNameQuery(userName);

            using (MySqlDataReader reader = sqlHelper.ExecuteQuery(query, _connectionString))
            {
                while (reader.Read())
                {
                    response.Roles.Add(SQLHelper.GetStringValue(reader, "RoleName"));
                }
            }
            if (!response.Roles.Any())
            {
                response.ResponseCode = 404;
                response.ResponseMessage = "User or Roles not found";
                return response;
            }

            response.UserName = userName;
            response.ResponseCode = 200;
            response.ResponseMessage = "Success";
            response.RecordCount = response.Roles.Count;
        }
        catch
        {
            throw;
        }
        finally
        {
            sqlHelper.CloseSqlConnection();
        }
        return response;
    }
    private string GenerateToken(User user, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("userName", user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        foreach (string role in roles)
        {
            //claims.Add(new Claim("role", role));
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer: _configuration["Jwt:Issuer"], audience: _configuration["Jwt:Audience"], claims: claims, expires: DateTime.UtcNow.AddHours(8), signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public async Task<UserProfileResponse> GetUserProfileAsync(UserProfileRequest request)
    {
        UserProfileResponse response = new UserProfileResponse();
        if (request == null || request.UserNames == null)
        {
            response.ResponseCode = 400;
            response.ResponseMessage = "Invalid Request";
            return response;
        }
        SQLHelper sqlHelper = new SQLHelper();
        AuthQueryGenerator queryGenerator = new AuthQueryGenerator();
        try
        {
            string query = queryGenerator.GetUserProfileQuery(request);
            using (MySqlDataReader reader = sqlHelper.ExecuteQuery(query, _connectionString))
            {
                while (reader.Read())
                {
                    UserProfile profile = new()
                    {
                        Id = SQLHelper.GetIntValue(reader, "Id"),
                        UserId = SQLHelper.GetStringValue(reader, "EntityId"),
                        FirstName = SQLHelper.GetStringValue(reader, "FirstName"),
                        LastName = SQLHelper.GetStringValue(reader, "LastName"),
                        UserName = SQLHelper.GetStringValue(reader, "UserName"),
                        Email = SQLHelper.GetStringValue(reader, "Email")
                    };

                    string roles = SQLHelper.GetStringValue(reader, "Roles");

                    profile.Roles = string.IsNullOrEmpty(roles) ? new List<string>() : roles.Split(',').ToList();

                    response.Users.Add(profile);
                }
            }
            if (!response.Users.Any())
            {
                response.ResponseCode = 404;
                response.ResponseMessage = "Users Not Found";
                response.RecordCount = 0;
            }
            else
            {
                response.ResponseCode = 200;
                response.ResponseMessage = "Success";
                response.RecordCount = response.Users.Count;
            }
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = "Internal Server Error";
        }
        finally
        {
            sqlHelper.CloseSqlConnection();
        }
        return response;
    }

    public async Task<GetRolesResponse> GetRolesAsync()
    {
        GetRolesResponse response = new();
        SQLHelper sqlHelper = new SQLHelper();
        AuthQueryGenerator queryGenerator = new AuthQueryGenerator();
        try
        {
            string query = queryGenerator.GetRolesQuery();

            using MySqlDataReader reader = sqlHelper.ExecuteQuery(query, _connectionString);

            while (reader.Read())
            {
                response.Roles.Add(SQLHelper.GetStringValue(reader, "Name"));
            }

            if (!response.Roles.Any())
            {
                response.ResponseCode = 404;
                response.ResponseMessage = "No roles found.";
                return response;
            }

            response.ResponseCode = 200;
            response.ResponseMessage = "Success";
            response.RecordCount = response.Roles.Count;
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = ex.Message;
        }
        finally
        {
            sqlHelper.CloseSqlConnection();
        }
        return response;
    }

    public async Task<BaseResponse> CreateRolesAsync(CreateRoleRequest request, string createdBy)
    {
        BaseResponse response = new();
        if (request == null)
        {
            response.ResponseCode = 400;
            response.ResponseMessage = "Invalid Role Request.";
            return response;
        }
        SQLHelper sqlHelper = new SQLHelper();
        AuthQueryGenerator queryGenerator = new AuthQueryGenerator();
        try
        {
            string query = queryGenerator.InsertRolesQuery(request.Roles, createdBy);

            int rowsAffected = sqlHelper.ExecuteNonQuery(query, _connectionString);

            if (rowsAffected > 0)
            {
                response.ResponseCode = 201;
                response.ResponseMessage = "Roles created successfully.";
                response.RecordCount = rowsAffected;
            }
            else
            {
                response.ResponseCode = 500;
                response.ResponseMessage = "Failed to create role.";
            }
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = "Internal Server Error";
        }
        finally
        {
            sqlHelper.CloseSqlConnection();
        }
        return response;
    }

    public async Task<BaseResponse> RemoveRoleAsync(string roleName)
    {
        BaseResponse response = new();
        if (string.IsNullOrWhiteSpace(roleName))
        {
            response.ResponseCode = 400;
            response.ResponseMessage = "Role Name is required.";

            return response;
        }

        SQLHelper sqlHelper = new SQLHelper();
        AuthQueryGenerator queryGenerator = new AuthQueryGenerator();
        try
        {
            string query = queryGenerator.RemoveRoleQuery(roleName);

            int rowsAffected = sqlHelper.ExecuteNonQuery(query, _connectionString);

            if (rowsAffected <= 0)
            {
                response.ResponseCode = 404;
                response.ResponseMessage = "Role not found.";
            }

            response.ResponseCode = 200;
            response.ResponseMessage = "Role removed successfully.";
            response.RecordCount = rowsAffected;
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = "Internal Server Error";
        }
        finally
        {
            sqlHelper.CloseSqlConnection();
        }
        return response;
    }

    public async Task<BaseResponse> ChangePasswordAsync(string userName, ChangePasswordRequest request)
    {
        BaseResponse response = ValidateChangePasswordRequest(request);

        if (response.ResponseCode != 200)
        {
            return response;
        }
        SQLHelper sqlHelper = new SQLHelper();
        AuthQueryGenerator queryGenerator = new AuthQueryGenerator();

        try
        {
            string query = queryGenerator.GetUserPasswordQuery(userName);

            string existingPasswordHash = string.Empty;

            using (MySqlDataReader reader = sqlHelper.ExecuteQuery(query,_connectionString))
            {
                if (reader.Read())
                {
                    existingPasswordHash = SQLHelper.GetStringValue(reader,"PasswordHash");
                }
            }

            if (string.IsNullOrEmpty(existingPasswordHash))
            {
                response.ResponseCode = 404;
                response.ResponseMessage = "User not found.";

                return response;
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, existingPasswordHash);

            if (!isValidPassword)
            {
                response.ResponseCode = 401;
                response.ResponseMessage = "Current password is incorrect.";

                return response;
            }

            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            string updateQuery = queryGenerator.UpdatePasswordQuery(userName, newPasswordHash);

            int rowsAffected = sqlHelper.ExecuteNonQuery(updateQuery, _connectionString);

            response.ResponseCode = rowsAffected > 0 ? 200 : 500;
            response.ResponseMessage = rowsAffected > 0 ? "Password changed successfully." : "Failed to update password.";
            response.RecordCount = rowsAffected;
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = "Internal Server Error";
        }
        finally
        {
            sqlHelper.CloseSqlConnection();
        }
        return response;
    }
    private BaseResponse ValidateChangePasswordRequest(ChangePasswordRequest request)
    {
        BaseResponse response = new();

        if (request == null)
        {
            response.ResponseCode = 400;
            response.ResponseMessage = "Request is required.";
            return response;
        }

        if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            response.ResponseCode = 400;
            response.ResponseMessage = "All Fields are required.";

            return response;
        }

        if (request.NewPassword != request.ConfirmPassword)
        {
            response.ResponseCode = 400;
            response.ResponseMessage = "Passwords do not match.";

            return response;
        }

        response.ResponseCode = 200;

        return response;
    }

    //public async Task<BaseResponse> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    //{
    //    BaseResponse response = new BaseResponse();

    //    if (request == null || userId == null)
    //    {
    //        response.ResponseCode = 400;
    //        response.ResponseMessage = "Invalid request.";
    //        return response;
    //    }
    //    SQLHelper sqlHelper = new SQLHelper();
    //    AuthQueryGenerator queryGenerator = new AuthQueryGenerator();

    //    try
    //    {

    //        int userNameCount = sqlHelper.ExecuteScalar(queryGenerator.GetUserQuery(userName: request.UserName, userId: userId), _connectionString);

    //        if (userNameCount > 0)
    //        {
    //            response.ResponseCode = 409;
    //            response.ResponseMessage = "Username already exists.";

    //            return response;
    //        }

    //        int rowsAffected = _sqlHelper.ExecuteNonQuery(_queryGenerator.UpdateProfileQuery(
    //                    userId,
    //                    request),
    //                _connectionString);

    //        response.ResponseCode =
    //            rowsAffected > 0 ? 200 : 500;

    //        response.ResponseMessage =
    //            rowsAffected > 0
    //                ? "Profile updated successfully."
    //                : "Failed to update profile.";

    //        response.RecordCount =
    //            rowsAffected;

    //        return response;
    //    }
    //    catch (Exception ex)
    //    {
    //        response.ResponseCode = 500;
    //        response.ResponseMessage =
    //            ex.Message;

    //        return response;
    //    }
    //}


}


